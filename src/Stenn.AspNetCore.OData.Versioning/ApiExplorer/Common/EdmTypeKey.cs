using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
    internal struct EdmTypeKey : IEquatable<EdmTypeKey>
    {
        private readonly int _hashCode;

        internal EdmTypeKey(IEdmStructuredType type, ApiVersion apiVersion)
        {
            _hashCode = ComputeHash(type.FullTypeName(), apiVersion);
        }

        internal EdmTypeKey(IEdmTypeReference type, ApiVersion apiVersion)
        {
            _hashCode = ComputeHash(type.FullName(), apiVersion);
        }

        internal EdmTypeKey(string fullTypeName, ApiVersion apiVersion)
        {
            _hashCode = ComputeHash(fullTypeName, apiVersion);
        }

        public static bool operator ==(EdmTypeKey obj, EdmTypeKey other)
        {
            return obj.Equals(other);
        }

        public static bool operator !=(EdmTypeKey obj, EdmTypeKey other)
        {
            return !obj.Equals(other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object? obj)
        {
            return obj is EdmTypeKey other && Equals(other);
        }

        public bool Equals(EdmTypeKey other)
        {
            return _hashCode == other._hashCode;
        }

        private static int ComputeHash(string fullName, ApiVersion apiVersion)
        {
            return HashCode.Combine(fullName, apiVersion);
        }
    }
}