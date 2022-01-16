#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class EdmModelMutator : IEdmModelMutator
    {
        private readonly ODataModelBuilder _builder;
        private readonly IEdmModelFilter[] _edmFilters;
        private readonly bool _requestModel;

        public EdmModelMutator(ODataModelBuilder builder, bool requestModel, IEnumerable<IEdmModelFilter> edmFilters)
        {
            _builder = builder;
            _requestModel = requestModel;
            _edmFilters = edmFilters.ToArray();
        }

        public void Run()
        {
            foreach (var edmType in _builder.StructuralTypes.ToList())
            {
                var edmSets = _builder.EntitySets.Where(s => s.EntityType == edmType).ToList();
                if (IsIgnored(edmType))
                {
                    edmSets.ForEach(set => _builder.RemoveEntitySet(set.Name));
                    _builder.RemoveStructuralType(edmType.ClrType);
                    continue;
                }

                foreach (var property in edmType.Properties.ToList())
                {
                    var ignoreTargetType = false;
                    var targetClrType = property switch
                    {
                        NavigationPropertyConfiguration navigationProperty => navigationProperty.RelatedClrType,
                        ComplexPropertyConfiguration complexProperty => complexProperty.RelatedClrType,
                        _ => null
                    };

                    if (targetClrType != null)
                    {
                        // targetType can be null if it has already been removed by a previous iteration of the outer loop
                        var targetType = _builder.StructuralTypes.SingleOrDefault(t => t.ClrType == targetClrType);
                        ignoreTargetType = targetType == null || IsIgnored(targetType);
                    }
                    if (ignoreTargetType || IsIgnored(property))
                    {
                        edmType.RemoveProperty(property.PropertyInfo);
                    }
                }

                foreach (var edmSet in edmSets)
                {
                    foreach (var binding in edmSet.Bindings.ToArray())
                    {
                        // when NavigationProperty is declared in the superclass, the corresponding Binding exists in all its child classes.
                        // so we have to use the DeclaringType property instead of "eType" while performing the Ignore check.
                        if (IsIgnored(binding.TargetNavigationSource.EntityType) ||
                            IsIgnored(binding.NavigationProperty))
                        {
                            edmSet.RemoveBinding(binding.NavigationProperty);
                        }
                    }
                }
            }

            foreach (var operation in _builder.Operations.ToArray())
            {
                if (IsIgnored(operation.ReturnType) ||
                    operation.Parameters.Any(p => IsIgnored(p.TypeConfiguration)))
                {
                    if (!_requestModel)
                    {
                        throw new InvalidOperationException(
                            $"Can't remove operation '{operation.Name}' from full edm model. The model used for odata name convention routing");
                    }
                    _builder.RemoveOperation(operation);
                }
            }
        }

        /// <inheritdoc />
        public bool IsIgnored(MemberInfo? memberInfo)
        {
            if (memberInfo is null)
            {
                return false;
            }
            
            for (var i = 0; i < _edmFilters.Length; i++)
            {
                var f = _edmFilters[i];
                if (f.IsIgnored(memberInfo))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsIgnored(IEdmTypeConfiguration? edmType)
        {
            switch (edmType)
            {
                case null:
                    return false;
                case CollectionTypeConfiguration collectionTypeConfiguration:
                    edmType = collectionTypeConfiguration.ElementType;
                    break;
            }
            if (edmType is PrimitiveTypeConfiguration)
            {
                return false;
            }
            return !_builder.StructuralTypes.Contains(edmType) || IsTypeIgnored(edmType);
        }

        public virtual bool IsIgnored(PropertyConfiguration? property)
        {
            if (property is null)
            {
                return false;
            }
            
            for (var i = 0; i < _edmFilters.Length; i++)
            {
                var f = _edmFilters[i];
                if (f.IsIgnored(property))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool IsTypeIgnored(IEdmTypeConfiguration type)
        {
            for (var i = 0; i < _edmFilters.Length; i++)
            {
                var f = _edmFilters[i];
                if (f.IsIgnored(type.ClrType) || f.IsIgnored(type))
                {
                    return true;
                }
            }
            return false;
        }
    }
}