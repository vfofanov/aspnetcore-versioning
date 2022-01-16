using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    /// <summary>
    ///     Skip properties if it marked <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute" /> or
    /// </summary>
    public sealed class NewtonsoftJsonIgnoreAttributeEdmFilter : IEdmModelFilter
    {
        /// <inheritdoc />
        public bool ForRequestModelOnly => true;

        /// <inheritdoc />
        public bool IsIgnored(MemberInfo memberInfo)
        {
            return memberInfo is not null && memberInfo.GetCustomAttributes<JsonIgnoreAttribute>(true).Any();
        }
    }
}