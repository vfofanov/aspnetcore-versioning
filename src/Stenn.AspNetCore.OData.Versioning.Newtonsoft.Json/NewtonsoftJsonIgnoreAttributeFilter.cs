using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    /// <summary>
    ///     Skip properties if it marked <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute" />
    /// </summary>
    public sealed class NewtonsoftJsonIgnoreAttributeFilter : DefaultModelKeyModelFilter
    {
        /// <inheritdoc />
        public override bool IsIgnored(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<JsonIgnoreAttribute>(true).Any();
        }
    }
}