using Microsoft.AspNetCore.OData.Routing;
using Microsoft.OData;

namespace Stenn.AspNetCore.OData.Versioning
{
    public class ODataVersioningOptions
    {
        /// <summary>
        ///     Version prefix template. By default '{0}/odata'
        /// </summary>
        public string VersionPrefixTemplate { get; set; } = "{0}/odata";

        /// <summary>
        ///     OData protocol version
        /// </summary>
        public ODataVersion ODataVersion { get; set; } = ODataVersion.V4;


        public ODataVersioningRouteOptions RouteOptions { get; set; } = new();
    }

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