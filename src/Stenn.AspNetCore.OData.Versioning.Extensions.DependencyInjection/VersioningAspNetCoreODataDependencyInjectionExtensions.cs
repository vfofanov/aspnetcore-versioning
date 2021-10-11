using System;
using System.Linq;
using AspNetCore.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection extensions for registee AspNet.Core OData versioning 
    /// </summary>
    public static class VersioningAspNetCoreODataDependencyInjectionExtensions
    {
        public static IServiceCollection AddVersioningODataCore<TODataConventionModelProvider, TODataModelProvider>(this IServiceCollection services)
            where TODataModelProvider : class, IODataModelProvider
            where TODataConventionModelProvider : class, IODataConventionModelProvider
        {
            services.TryAddSingleton<TODataModelProvider>();
            services.TryAddSingleton<TODataConventionModelProvider>();

            services.TryAddSingleton<IODataModelProvider>(p => p.GetService<TODataModelProvider>());
            services.TryAddSingleton<IODataConventionModelProvider>(p => p.GetService<TODataConventionModelProvider>());

            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ODataVersioningRoutingApplicationModelProvider>());

            //Options
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ODataVersioningOptions>, ODataVersioningOptionsSetup>());

            return services;
        }

        /// <summary>
        /// Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// /// <param name="versioningSetupAction">The OData versioning options to configure the services with,
        /// including access to a service provider which you can resolve services from.</param>
        /// <param name="setupAction">The OData options to configure the services with,
        /// including access to a service provider which you can resolve services from.</param>
        /// <returns>A <see cref="IMvcBuilder"/> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TODataModelProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction)
            where TODataModelProvider : class, IODataModelProvider, IODataConventionModelProvider
        {
            return builder.AddVersioningOData<TODataModelProvider, TODataModelProvider>(
                (options, _) => versioningSetupAction?.Invoke(options),
                (options, _) => setupAction?.Invoke(options));
        }

        /// <summary>
        /// Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// /// <param name="versioningSetupAction">The OData versioning options to configure the services with,
        /// including access to a service provider which you can resolve services from.</param>
        /// <param name="setupAction">The OData options to configure the services with,
        /// including access to a service provider which you can resolve services from.</param>
        /// <returns>A <see cref="IMvcBuilder"/> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TODataModelProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction)
            where TODataModelProvider : class, IODataModelProvider, IODataConventionModelProvider
        {
            return builder.AddVersioningOData<TODataModelProvider, TODataModelProvider>(versioningSetupAction, setupAction);
        }

        /// <summary>
        /// Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// /// <param name="versioningSetupAction">The OData versioning options to configure the services with,
        /// including access to a service provider which you can resolve services from.</param>
        /// <param name="setupAction">The OData options to configure the services with,
        /// including access to a service provider which you can resolve services from.</param>
        /// <returns>A <see cref="IMvcBuilder"/> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TODataConventionModelProvider, TODataModelProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction)
            where TODataModelProvider : class, IODataModelProvider
            where TODataConventionModelProvider : class, IODataConventionModelProvider
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddVersioningODataCore<TODataConventionModelProvider, TODataModelProvider>();
            builder.Services.AddOptions<ODataVersioningOptions>().Configure(versioningSetupAction);

            builder.AddOData((options, provider) =>
            {
                options.ConfigureVersioningOData(provider);
                setupAction?.Invoke(options, provider);
            });

            return builder;
        }

        /// <summary>
        /// Configures versioned odata controllers, use it inside method <see cref="ODataMvcBuilderExtensions.AddOData(IMvcBuilder,Action{ODataOptions, IServiceProvider})"/>
        /// </summary>
        /// <param name="options">OData options</param>
        /// <param name="provider">Service provider</param>
        public static void ConfigureVersioningOData(this ODataOptions options, IServiceProvider provider)
        {
            //NOTE:Replace metadata convension
            options.Conventions.Remove(options.Conventions.OfType<MetadataRoutingConvention>().First());
            options.Conventions.Add(new VersionedMetadataRoutingConvention<MetadataController>());

            var apiVersionsProvider = provider.GetRequiredService<IApiVersionInfoProvider>();
            var modelProvider = provider.GetRequiredService<IODataConventionModelProvider>();
            var versioningOptions = provider.GetRequiredService<IOptions<ODataVersioningOptions>>();
            var prefixTemplate = versioningOptions.Value.VersionPrefixTemplate;

            foreach (var version in apiVersionsProvider.Versions)
            {
                var prefix = VersioningRoutingPrefixHelper.GeneratePrefix(prefixTemplate, version).TrimStart('/');
                options.AddRouteComponents(prefix, modelProvider.GetNameConventionEdmModel(version.Version));
            }
        }
    }
}