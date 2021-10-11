using System;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Options;

namespace AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    /// Sets up default options for <see cref="ODataVersioningOptions"/>.
    /// </summary>
    public class ODataVersioningOptionsSetup : IConfigureOptions<ODataVersioningOptions>
    {
        /// <summary>
        /// Configure the default <see cref="ODataOptions"/>
        /// </summary>
        /// <param name="options">The OData options.</param>
        public void Configure(ODataVersioningOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.VersionPrefixTemplate = "{0}/odata";
        }
    }
}