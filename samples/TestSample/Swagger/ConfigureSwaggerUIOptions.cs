using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Stenn.AspNetCore.Versioning;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace TestSample.Swagger
{
    public class ConfigureSwaggerUIOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerUIOptions"/> class.
        /// </summary>
        public ConfigureSwaggerUIOptions(IApiVersionInfoProvider versionInfoProvider)
        {
            _versionInfoProvider = versionInfoProvider
                                   ?? throw new ArgumentNullException(nameof(versionInfoProvider));
        }

        public void Configure(SwaggerUIOptions options)
        {
            foreach (var apiVersion in _versionInfoProvider.Versions)
            {
                var name = apiVersion.RoutePathName;
                options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
            }
            options.RoutePrefix = string.Empty;
            options.DocExpansion(DocExpansion.None);
        }
    }
}