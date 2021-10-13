using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Defines the behavior of an OData query options convention.
    /// </summary>
    public interface IODataQueryOptionsConvention
    {
        /// <summary>
        ///     Applies the convention to the specified API description.
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription">API description</see> to apply the convention to.</param>
        void ApplyTo(ApiDescription apiDescription);
    }
}