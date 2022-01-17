#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class ApiVersionEdmModelFilter : IEdmModelFilter
    {
        public ApiVersionEdmModelFilter(ApiVersion apiVersion)
        {
            ApiVersion = apiVersion;
        }

        public ApiVersion ApiVersion { get; }

        /// <inheritdoc />
        public bool ForRequestModelOnly => false;
        
        /// <inheritdoc />
        public EdmModelKey GetKey()
        {
            return EdmModelKey.Get(ApiVersion);
        }

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