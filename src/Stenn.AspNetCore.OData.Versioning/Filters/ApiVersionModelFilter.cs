#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class ApiVersionModelFilter : IModelFilter
    {
        public ApiVersionModelFilter(ApiVersion apiVersion)
        {
            ApiVersion = apiVersion;
        }

        public ApiVersion ApiVersion { get; }

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

        private bool Match(ICustomAttributeProvider memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes(true);
            var match = true;
            
            for (var i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute is not IApiVersionProvider provider)
                {
                    continue;
                }
                if (provider.Versions.Contains(ApiVersion))
                {
                    return true;
                }
                match = false;
            }
            return match;
        }
    }
}