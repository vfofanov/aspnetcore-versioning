using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Default versioning route prefix provider
    /// </summary>
    public sealed class DefaultVersioningRoutingPrefixProvider : IVersioningRoutingPrefixProvider
    {
        private readonly Func<ControllerModel, string> _prefixFormatTemplateFunc;

        public DefaultVersioningRoutingPrefixProvider(Func<ControllerModel, string> prefixFormatTemplateFunc)
        {
            _prefixFormatTemplateFunc = prefixFormatTemplateFunc ?? throw new ArgumentNullException(nameof(prefixFormatTemplateFunc));
        }

        /// <inheritdoc />
        public string GetPrefix(ControllerModel controller, ApiVersionInfo version)
        {
            var template = _prefixFormatTemplateFunc(controller);
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
            return VersioningRoutingPrefixHelper.GeneratePrefix(template, version);
        }
    }
}