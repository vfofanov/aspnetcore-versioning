using System;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Options;
using Stenn.AspNetCore.OData.Versioning.ApiExplorer;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    ///     Sets up default options for <see cref="ODataVersioningOptions" />.
    /// </summary>
    public class ODataVersioningApiExplorerOptionsSetup : IConfigureOptions<ODataVersioningApiExplorerOptions>
    {
        /// <summary>
        ///     Configure the default <see cref="ODataOptions" />
        /// </summary>
        /// <param name="options">The OData options.</param>
        public void Configure(ODataVersioningApiExplorerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
        }
    }
}