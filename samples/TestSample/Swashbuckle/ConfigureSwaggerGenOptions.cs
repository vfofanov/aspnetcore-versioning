using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Stenn.AspNetCore.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TestSample.Swashbuckle
{
    public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerGenOptions"/> class.
        /// </summary>
        public ConfigureSwaggerGenOptions(IApiVersionInfoProvider versionInfoProvider)
        {
            _versionInfoProvider = versionInfoProvider
                                   ?? throw new ArgumentNullException(nameof(versionInfoProvider));
        }

        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var info in _versionInfoProvider.Versions)
            {
                options.SwaggerDoc(info.RoutePathName, CreateInfoForApiVersion(info));
            }

            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();
            
            options.OrderActionsBy(apiDesc => apiDesc.RelativePath);

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        public static OpenApiInfo CreateInfoForApiVersion(ApiVersionInfo versionInfo)
        {
            var info = new OpenApiInfo
            {
                Title = "Test API",
                Version = versionInfo.Version.ToString()
            };

            if (versionInfo.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}