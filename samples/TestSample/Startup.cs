using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using Newtonsoft.Json.Converters;
using Stenn.AspNetCore.OData.Versioning.CsvRouting;
using Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection;
using Stenn.AspNetCore.Versioning.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using TestSample.Controllers;
using TestSample.Controllers.OData;
using TestSample.Models.OData;
using TestSample.Swagger;

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
                .AddVersioningOData<MetadataController, ODataModelProvider>(versioningOptions =>
                    {
                        versioningOptions.RouteOptions.EnableEntitySetCount = false;

                        versioningOptions.ODataVersion = ODataVersion.V4;
                        versioningOptions.VersionPrefixTemplate = "api/{0}/odata";
                    },
                    options =>
                    {
                        options.EnableAttributeRouting = false;
                        options.RouteOptions.EnableKeyAsSegment = false;
                        options.RouteOptions.EnableControllerNameCaseInsensitive = true;
                        options.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;

                        options.EnableQueryFeatures();
                    });

            //Swagger
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddSwaggerGen();
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

            app.UseSwagger();
            app.UseSwaggerUI();

            #region ReDoc
            {
                //NOTE: Make different routes for different versions
                var swaggerUIOptions = app.ApplicationServices.GetRequiredService<IOptions<SwaggerUIOptions>>().Value;
                foreach (var url in swaggerUIOptions.ConfigObject.Urls)
                {
                    var versionName = url.Name;
                    app.UseReDoc(c =>
                    {
                        c.DocumentTitle = $"OData 8 Versioning Sample ({versionName})";
                        c.RoutePrefix = $"api-docs/{versionName}";
                        c.SpecUrl(url.Url);

                        c.EnableUntrustedSpec();
                        c.ScrollYOffset(10);
                        c.HideHostname();
                        c.HideDownloadButton();
                        c.ExpandResponses("200,201");
                        c.RequiredPropsFirst();
                        c.NoAutoAuth();
                        c.PathInMiddlePanel();
                        c.HideLoading();
                        c.NativeScrollbars();
                        c.DisableSearch();
                        c.OnlyRequiredInSamples();
                        c.SortPropsAlphabetically();
                    });
                }
            }
            #endregion

            app.UseODataCsvDocs();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}