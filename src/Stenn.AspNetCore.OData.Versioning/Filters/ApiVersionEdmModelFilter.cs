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

        /// <inheritdoc />
        void IVersioningEdmModelFilter.SetVersion(ApiVersion version)
        {
            _apiVersion = version;
        }
        private ApiVersion ApiVersion => _apiVersion ?? throw new NullReferenceException("ApiVersion is null. Use SetVersion first");

        /// <inheritdoc />
        public bool ForRequestModelOnly => true;

        /// <inheritdoc />
        public bool IsIgnored(MemberInfo memberInfo)
        {
            while (true)
            {
                if (memberInfo is null)
                {
                    return false;
                }
                if (Match(memberInfo))
                {
                    return false;
                }
                
                switch (memberInfo)
                {
                    case PropertyInfo propertyInfo:
                        memberInfo = propertyInfo.DeclaringType;
                        continue;
                    case MethodInfo methodInfo:
                        memberInfo = methodInfo.DeclaringType;
                        continue;
                }
                return true;
            }
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