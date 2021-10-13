using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNet.OData
{
    [DebuggerDisplay("{Name,nq} ({ApiVersion,nq})")]
    internal sealed class ClassSignature : IEquatable<ClassSignature>
    {
        private static readonly ConstructorInfo NewOriginalType = typeof(OriginalTypeAttribute).GetConstructors()[0];
        private static readonly CustomAttributeBuilder[] NoAttributes = Array.Empty<CustomAttributeBuilder>();
        private readonly Lazy<int> _hashCode;

        internal ClassSignature(Type originalType, IEnumerable<ClassProperty> properties, ApiVersion apiVersion)
        {
            var attributeBuilders = new List<CustomAttributeBuilder>
            {
                new(NewOriginalType, new object[] { originalType })
            };

            attributeBuilders.AddRange(originalType.DeclaredAttributes());

            Name = originalType.FullName!;
            Attributes = attributeBuilders.ToArray();
            Properties = properties.ToArray();
            ApiVersion = apiVersion;
            _hashCode = new Lazy<int>(ComputeHashCode);
        }

        internal ClassSignature(string name, IEnumerable<ClassProperty> properties, ApiVersion apiVersion)
        {
            Name = name;
            Attributes = NoAttributes;
            Properties = properties.ToArray();
            ApiVersion = apiVersion;
            _hashCode = new Lazy<int>(ComputeHashCode);
        }

        internal string Name { get; }

        internal IReadOnlyList<CustomAttributeBuilder> Attributes { get; }

        internal ClassProperty[] Properties { get; }

        internal ApiVersion ApiVersion { get; }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is ClassSignature s && Equals(s);
        }

        public bool Equals(ClassSignature? other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        private int ComputeHashCode()
        {
            if (Properties.Length == 0)
            {
                return 0;
            }

            ref var property = ref Properties[0];
            var hash = property.GetHashCode();

            for (var i = 1; i < Properties.Length; i++)
            {
                property = ref Properties[i];
                hash = (hash * 397) ^ Properties[i].GetHashCode();
            }

            return hash;
        }
    }
}