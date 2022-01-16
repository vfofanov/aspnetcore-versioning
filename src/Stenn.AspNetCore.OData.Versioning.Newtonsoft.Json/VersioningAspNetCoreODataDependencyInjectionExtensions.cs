using Newtonsoft.Json;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for registee AspNet.Core OData versioning
    /// </summary>
    public static class VersioningAspNetCoreODataDependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds supports for ignore properties marked with <see cref="JsonIgnoreAttribute" />
        /// </summary>
        /// <param name="builder">The <see cref="EdmFilterBuilder" /> to add filters to.</param>
        /// <returns>A <see cref="EdmFilterBuilder" /> that can be used to further configure the filters.</returns>
        public static EdmFilterBuilder AddNewtonsoftJson(this EdmFilterBuilder builder)
        {
            return builder.AddWithDefaultModelKey<NewtonsoftJsonIgnoreAttributeEdmFilter>();
        }
    }
}