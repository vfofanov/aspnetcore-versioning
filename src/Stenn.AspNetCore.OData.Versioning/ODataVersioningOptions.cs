﻿using Microsoft.OData;

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
}