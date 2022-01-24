using System.Reflection;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace TestSample
{
    public class DenyAllEdmFilter : IEdmModelFilter
    {
        /// <inheritdoc />
        public EdmModelKey GetKey()
        {
            return EdmModelKey.Default;
        }

        /// <inheritdoc />
        public bool ForRequestModelOnly => true;

        /// <inheritdoc />
        public bool IsIgnored(MemberInfo memberInfo)
        {
            return true;
        }
    }
}