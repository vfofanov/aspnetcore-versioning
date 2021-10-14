using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using Newtonsoft.Json.Converters;
using Stenn.AspNetCore.OData.Versioning;
using Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection;
using Stenn.AspNetCore.Versioning;
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
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddVersioningOData<MetadataController, ODataModelProvider>(versioningOptions =>
                    {
                        versioningOptions.ODataVersion = ODataVersion.V4;
                        versioningOptions.VersionPrefixTemplate = "api/{0}/odata";
                    },
                    options =>
                    {
                        options.EnableQueryFeatures();
                    });
            
            //Swagger
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddSwaggerGen();
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
            app.UseSwaggerUI(options =>
            {
                var versionProvider = app.ApplicationServices.GetRequiredService<IApiVersionInfoProvider>();
                foreach (var apiVersion in versionProvider.Versions)
                {
                    var name = apiVersion.RoutePathName;
                    options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
                }
                options.RoutePrefix = string.Empty;
                options.DocExpansion(DocExpansion.None);
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}