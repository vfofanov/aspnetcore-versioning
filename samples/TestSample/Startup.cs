using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using Newtonsoft.Json.Converters;
using NSwag.AspNetCore;
using Stenn.AspNetCore.OData.Versioning;
using Stenn.AspNetCore.OData.Versioning.CsvRouting;
using Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection;
using Stenn.AspNetCore.Versioning;
using Stenn.AspNetCore.Versioning.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using TestSample.Controllers;
using TestSample.Models.OData;
using TestSample.Swashbuckle;
using MetadataController = TestSample.Controllers.OData.MetadataController;

namespace TestSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BookStoreContext>(opt => opt.UseInMemoryDatabase("BookLists"));
            
            services.AddVersioningForApi<ApiVersionInfoProviderFactory>(controller =>
            {
                if (controller.ControllerType.IsAssignableTo(typeof(BackOfficeController)))
                {
                    return "api/backOffice/{0}";
                }
                return "api/{0}";
            });

            services.AddControllers()
                .AddNewtonsoftJson(options => { options.SerializerSettings.Converters.Add(new StringEnumConverter()); })
                .AddVersioningODataModelPerRequest<MetadataController, EdmModelFactory>(
                    versioningOptions =>
                    {
                        versioningOptions.RouteOptions.EnableEntitySetCount = false;

                        versioningOptions.ODataVersion = ODataVersion.V4;
                        versioningOptions.VersionPrefixTemplate = "api/{0}/odata";
                    },
                    options =>
                    {
                        options.EnableAttributeRouting = false;
                        options.RouteOptions.EnableKeyInParenthesis = false;
                        options.RouteOptions.EnableKeyAsSegment = true;
                        options.RouteOptions.EnableControllerNameCaseInsensitive = true;
                        options.RouteOptions.EnableQualifiedOperationCall = false;
                        options.EnableQueryFeatures();
                    },
                    edmModelFilterBuilder =>
                    {
                        edmModelFilterBuilder.AddNewtonsoftJsonIgnore();
                        //edmModelFilterBuilder.Add<DenyAllEdmFilter>();
                    });

            services.AddVersioningODataApiExplorer();
            
            AddSwagbuckle(services);

            AddNSwag(services);
        }

        private static void AddNSwag(IServiceCollection services)
        {
            using var provider = services.BuildServiceProvider();
            var versionInfoProvider = provider.GetRequiredService<IApiVersionInfoProvider>();
                
            foreach (var version in versionInfoProvider.Versions)
            {
                var versionTmp = version;
                services.AddOpenApiDocument(settings =>
                {
                    settings.UseXmlDocumentation = true;
                    settings.UseHttpAttributeNameAsOperationId = false;
                    settings.UseControllerSummaryAsTagDescription = true;
                    settings.Title = "Test API";
                    settings.Version = versionTmp.Version.ToString();
                    settings.DocumentName = versionTmp.RoutePathName;
                    settings.ApiGroupNames = new[] { versionTmp.RoutePathName };
                });
            }
        }

        private static void AddSwagbuckle(IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddTransient<IConfigureOptions<SwaggerOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                var defaultSelector = options.SwaggerGeneratorOptions.OperationIdSelector;
                options.CustomOperationIds(apiDesc =>
                {
                    if (apiDesc.ActionDescriptor is not ControllerActionDescriptor actionDesc)
                    {
                        return defaultSelector(apiDesc);
                    }
                    var controller = actionDesc.ControllerTypeInfo;
                    var prefix = string.Empty;

                    if (controller.IsAssignableTo(typeof(ApiController)))
                    {
                        if (controller.IsAssignableTo(typeof(BackOfficeController)))
                        {
                            prefix = "backOffice";
                        }
                    }
                    else if (controller.IsAssignableTo(typeof(ODataController)))
                    {
                        prefix = "odata";
                    }
                    else
                    {
                        return defaultSelector(apiDesc);
                    }

                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefix += "_";
                    }

                    return $"{prefix}{actionDesc.ControllerName}_{actionDesc.ActionName}";
                });
            });
            //SwaggerUI
            services.AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUIOptions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseODataRouteDebug(); //OData debugging endpoints page 
            }

            app.UseRouting();
            app.UseODataUnauthorizedHandler();
            
            UseSwashbuckle(app);

            UseNSwag(app);

            #region ReDoc
            // {
            //     //NOTE: Make different routes for different versions
            //     var swaggerUIOptions = app.ApplicationServices.GetRequiredService<IOptions<SwaggerUIOptions>>().Value;
            //     foreach (var url in swaggerUIOptions.ConfigObject.Urls)
            //     {
            //         var versionName = url.Name;
            //         app.UseReDoc(c =>
            //         {
            //             c.DocumentTitle = $"OData 8 Versioning Sample ({versionName})";
            //             c.RoutePrefix = $"api-docs/{versionName}";
            //             c.SpecUrl(url.Url);
            //
            //             c.EnableUntrustedSpec();
            //             c.ScrollYOffset(10);
            //             c.HideHostname();
            //             c.HideDownloadButton();
            //             c.ExpandResponses("200,201");
            //             c.RequiredPropsFirst();
            //             c.NoAutoAuth();
            //             c.PathInMiddlePanel();
            //             c.HideLoading();
            //             c.NativeScrollbars();
            //             c.DisableSearch();
            //             c.OnlyRequiredInSamples();
            //             c.SortPropsAlphabetically();
            //         });
            //     }
            // }
            #endregion

            app.UseODataCsvDocs();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void UseNSwag(IApplicationBuilder app)
        {
            var docPath = "/nswag/{0}/swagger.json";
            app.UseOpenApi(settings =>
            {
                settings.Path = string.Format(docPath, "{documentName}");
                settings.PostProcess = (document, request) =>
                {
                    document.Servers.Clear();
                };
            });
            app.UseSwaggerUi3(settings =>
            {
                settings.Path = "/nswag-ui";
                var versionInfoProvider = app.ApplicationServices.GetRequiredService<IApiVersionInfoProvider>();
                foreach (var apiVersion in versionInfoProvider.Versions)
                {
                    var documentName = apiVersion.RoutePathName;
                    settings.SwaggerRoutes.Add(new SwaggerUi3Route(documentName, string.Format(docPath, documentName)));
                }
            });
        }

        private static void UseSwashbuckle(IApplicationBuilder app)
        {
            // ReSharper disable once RedundantNameQualifier
            Microsoft.AspNetCore.Builder.SwaggerBuilderExtensions.UseSwagger(app);
            app.UseSwaggerUI(options => { options.RoutePrefix = "swashbuckle-ui"; });
        }
    }
}