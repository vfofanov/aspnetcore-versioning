using Microsoft.AspNet.OData.Builder;

namespace Stenn.AspNetCore.OData.Versioning.ApiExplorer
{
    public sealed class ODataVersioningApiExplorerOptions
    {
        /// <summary>
        ///     Convention builder for build query parameters in ApiExplorer
        /// </summary>
        public ODataQueryOptionsConventionBuilder QueryOptions { get; set; } = new();
    }
}