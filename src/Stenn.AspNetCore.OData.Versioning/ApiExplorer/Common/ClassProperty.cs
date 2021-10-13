using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
    [DebuggerDisplay("{Name,nq}")]
    internal readonly struct ClassProperty
    {
        internal readonly Type Type;
        internal readonly string Name;

        internal ClassProperty(PropertyInfo clrProperty, Type propertyType)
        {
            Name = clrProperty.Name;
            Type = propertyType;
            Attributes = clrProperty.DeclaredAttributes().ToArray();
        }

        internal ClassProperty(IEdmOperationParameter parameter, TypeSubstitutionContext context)
        {
            Name = parameter.Name;

            if (parameter.Type.IsCollection())
            {
                var collectionType = parameter.Type.AsCollection();
                var elementType = collectionType.ElementType().Definition.GetClrType(context.Model)!;
                var substitutedType = elementType.SubstituteIfNecessary(context);

                Type = typeof(IEnumerable<>).MakeGenericType(substitutedType);
            }
            else
            {
                var parameterType = parameter.Type.Definition.GetClrType(context.Model)!;

                Type = parameterType.SubstituteIfNecessary(context);
            }

            Attributes = AttributesFromOperationParameter(parameter).ToArray();
        }

        internal IReadOnlyList<CustomAttributeBuilder> Attributes { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }
        private static IEnumerable<CustomAttributeBuilder> AttributesFromOperationParameter(IEdmOperationParameter parameter)
        {
            if (parameter.Type.IsNullable)
            {
                yield break;
            }

            var ctor = typeof(RequiredAttribute).GetConstructors().Single(c => c.GetParameters().Length == 0);
            var args = Array.Empty<object>();

            yield return new CustomAttributeBuilder(ctor, args);
        }
    }
}