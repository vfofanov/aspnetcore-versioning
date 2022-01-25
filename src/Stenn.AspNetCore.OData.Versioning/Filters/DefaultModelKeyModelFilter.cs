using System.Reflection;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    /// <summary>
    ///     Base edm model filter with default model key. This filter will be added like singleton
    /// </summary>
    public abstract class DefaultModelKeyModelFilter : IModelFilter
    {
        /// <inheritdoc />
        public abstract bool IsIgnored(MemberInfo memberInfo);

        /// <inheritdoc />
        public EdmModelKey GetKey() => EdmModelKey.Default;
    }
}