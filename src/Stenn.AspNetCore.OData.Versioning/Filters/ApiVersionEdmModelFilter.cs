#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class ApiVersionEdmModelFilter : IVersioningEdmModelFilter
    {
        private ApiVersion? _apiVersion;
        private ApiVersion ApiVersion => _apiVersion ?? throw new NullReferenceException("ApiVersion is null. Use SetVersion first");

        /// <inheritdoc />
        public EdmModelKey GetKey()
        {
            return EdmModelKey.Get(ApiVersion);
        }

        /// <inheritdoc />
        void IVersioningEdmModelFilter.SetVersion(ApiVersion version)
        {
            _apiVersion = version;
        }

        /// <inheritdoc />
        public bool ForRequestModelOnly => true;

        /// <inheritdoc />
        public bool IsIgnored(MemberInfo memberInfo)
        {
            var declaringType = memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.DeclaringType,
                MethodInfo methodInfo => methodInfo.DeclaringType,
                _ => null
            };
            return declaringType is not null && !Match(declaringType) ||
                   !Match(memberInfo);
        }

        private bool Match(MemberInfo memberInfo)
        {
            var match = true;
            foreach (var apiVersionAttribute in memberInfo.GetCustomAttributes<ApiVersionAttribute>(true))
            {
                if (apiVersionAttribute.Versions.Contains(ApiVersion))
                {
                    return true;
                }
                match = false;
            }
            return match;
        }
    }
}