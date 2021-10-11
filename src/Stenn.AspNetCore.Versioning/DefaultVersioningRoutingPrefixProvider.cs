using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCore.Versioning
{
    /// <summary>
    /// Default versioning route prefix provider
    /// </summary>
    public class DefaultVersioningRoutingPrefixProvider : IVersioningRoutingPrefixProvider
    {
        private readonly string _prefixFormatTemplate;

        public DefaultVersioningRoutingPrefixProvider(string prefixFormatTemplate = "{0}")
        {
            if (string.IsNullOrWhiteSpace(prefixFormatTemplate))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(prefixFormatTemplate));
            }

            _prefixFormatTemplate = prefixFormatTemplate;
        }

        /// <inheritdoc />
        public virtual string GetPrefix(ControllerModel controller, ApiVersionInfo version)
        {
            return VersioningRoutingPrefixHelper.GeneratePrefix(_prefixFormatTemplate, version);
        }
    }
}