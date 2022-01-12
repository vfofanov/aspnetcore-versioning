using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Stenn.AspNetCore.Versioning;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace TestSample.Swashbuckle
{
    public class ConfigureSwaggerUIOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;
        private readonly IOptions<SwaggerOptions> _swaggerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerUIOptions"/> class.
        /// </summary>
        public ConfigureSwaggerUIOptions(IApiVersionInfoProvider versionInfoProvider, IOptions<SwaggerOptions> swaggerOptions)
        {
            _versionInfoProvider = versionInfoProvider
                                   ?? throw new ArgumentNullException(nameof(versionInfoProvider));
            _swaggerOptions = swaggerOptions;
        }

        public void Configure(SwaggerUIOptions options)
        {
            var rootTemplate = _swaggerOptions.Value.RouteTemplate
                .Replace("{documentName}", "{0}")
                .Replace("{json|yaml}", "{1}");

            foreach (var apiVersion in _versionInfoProvider.Versions)
            {
                var name = apiVersion.RoutePathName;

                options.SwaggerEndpoint(string.Format(rootTemplate, name, "json"), name);
            }
            
            options.EnableFilter();
            options.DocExpansion(DocExpansion.None);
        }
    }
}