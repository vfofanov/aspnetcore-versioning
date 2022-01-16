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
            foreach (var eType in _builder.StructuralTypes.ToList())
            {
                var eSets = _builder.EntitySets.Where(s => s.EntityType == eType).ToList();
                if (IsIgnored(eType))
                {
                    eSets.ForEach(set => _builder.RemoveEntitySet(set.Name));
                    _builder.RemoveStructuralType(eType.ClrType);
                    continue;
                }

                foreach (var property in eType.Properties.ToList())
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
                        eType.RemoveProperty(property.PropertyInfo);
                    }
                }

                foreach (var eSet in eSets)
                {
                    foreach (var binding in eSet.Bindings.ToArray())
                    {
                        // when NavigationProperty is declared in the superclass, the corresponding Binding exists in all its child classes.
                        // so we have to use the DeclaringType property instead of "eType" while performing the Ignore check.
                        if (IsIgnored(binding.TargetNavigationSource.EntityType) ||
                            IsIgnored(binding.NavigationProperty))
                        {
                            eSet.RemoveBinding(binding.NavigationProperty);
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
        public bool IsIgnored(MemberInfo memberInfo)
        {
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

        public virtual bool IsIgnored(IEdmTypeConfiguration edmType)
        {
            if (edmType is CollectionTypeConfiguration collectionTypeConfiguration)
            {
                edmType = collectionTypeConfiguration.ElementType;
            }
            if (edmType is PrimitiveTypeConfiguration)
            {
                return false;
            }
            return !_builder.StructuralTypes.Contains(edmType) || IsTypeIgnored(edmType);
        }

        public virtual bool IsIgnored(PropertyConfiguration property)
        {
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