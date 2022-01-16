using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    /// <summary>
    ///     Skip properties if it marked <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute" />
    /// </summary>
    public sealed class TextJsonIgnoreAttributeEdmModelFilter : DefaultModelKeyEdmModelFilter
    {
        /// <inheritdoc />
        public override bool IsIgnored(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<JsonIgnoreAttribute>(true).Any();
        }
    }
}