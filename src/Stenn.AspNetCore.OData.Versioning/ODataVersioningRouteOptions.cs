using Microsoft.AspNetCore.OData.Routing;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    /// OData route options for addition to <see cref="ODataRouteOptions"/>
    /// </summary>
    public class ODataVersioningRouteOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to generate odata path template as ~/entityset/$count
        /// Used in conventional routing.
        /// </summary>
        public bool EnableEntitySetCount { get; set; } = true;
    }
}