using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for registee AspNet.Core OData versioning
    /// </summary>
    public static class VersioningAspNetCoreODataDependencyInjectionExtensions
    {
        public static IServiceCollection AddVersioningODataCore<TODataModelProvider, TODataModelRequestProvider>(this IServiceCollection services)
            where TODataModelProvider : class, IODataModelProvider
            where TODataModelRequestProvider : class, IODataModelRequestProvider
        {
            services.AddVersioningODataCore<TODataModelProvider>();

            services.TryAddSingleton<IODataModelRequestProvider, TODataModelRequestProvider>();
            services.TryAddSingleton<IODataModelRequestProvider>(p => p.GetRequiredService<TODataModelRequestProvider>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, VersioningODataRoutingMatcherPolicy>());

            return services;
        }

        public static IServiceCollection AddVersioningODataCore<TODataModelProvider>(this IServiceCollection services)
            where TODataModelProvider : class, IODataModelProvider
        {
            services.TryAddSingleton<IODataModelProvider, TODataModelProvider>();
            services.TryAddSingleton<IODataModelProvider>(p => p.GetRequiredService<TODataModelProvider>());

            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ODataVersioningRoutingApplicationModelProvider>());

            //Options
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ODataVersioningOptions>, ODataVersioningOptionsSetup>());

            //For ApiExplorer
            services.TryAddSingleton<IEdmModelSelector, EdmModelSelector>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, ODataQueryParametersApiDescriptionProvider>());

            return services;
        }

        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// ///
        /// <param name="versioningSetupAction">
        ///     The OData versioning options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <param name="setupAction">
        ///     The OData options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TODataModelProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction)
            where TMetadataController : MetadataControllerBase
            where TODataModelProvider : class, IODataModelProvider
        {
            return builder.AddVersioningOData<TMetadataController, TODataModelProvider>(
                (options, _) => versioningSetupAction(options),
                (options, _) => setupAction(options));
        }

        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// ///
        /// <param name="versioningSetupAction">
        ///     The OData versioning options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <param name="setupAction">
        ///     The OData options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TODataModelProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction)
            where TMetadataController : MetadataControllerBase
            where TODataModelProvider : class, IODataModelProvider
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddVersioningODataCore<TODataModelProvider>();
            builder.Services.AddOptions<ODataVersioningOptions>().Configure(versioningSetupAction);

            builder.AddOData((options, provider) =>
            {
                ConfigureVersioningOData<TMetadataController>(options, provider);
                setupAction(options, provider);
            });

            return builder;
        }

        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// ///
        /// <param name="versioningSetupAction">
        ///     The OData versioning options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <param name="setupAction">
        ///     The OData options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TODataModelProvider, TODataModelRequestProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction)
            where TMetadataController : MetadataControllerBase
            where TODataModelProvider : class, IODataModelProvider
            where TODataModelRequestProvider : class, IODataModelRequestProvider
        {
            return builder.AddVersioningOData<TMetadataController, TODataModelProvider, TODataModelRequestProvider>(
                (options, _) => versioningSetupAction(options),
                (options, _) => setupAction(options));
        }

        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// ///
        /// <param name="versioningSetupAction">
        ///     The OData versioning options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <param name="setupAction">
        ///     The OData options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TODataModelProvider, TODataModelRequestProvider>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction)
            where TMetadataController : MetadataControllerBase
            where TODataModelProvider : class, IODataModelProvider
            where TODataModelRequestProvider : class, IODataModelRequestProvider
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddVersioningODataCore<TODataModelProvider, TODataModelRequestProvider>();
            builder.Services.AddOptions<ODataVersioningOptions>().Configure(versioningSetupAction);

            builder.AddOData((options, provider) =>
            {
                ConfigureVersioningOData<TMetadataController>(options, provider);
                setupAction(options, provider);
            });

            return builder;
        }

        /// <summary>
        ///     Configures versioned odata controllers, use it inside method
        ///     <see cref="ODataMvcBuilderExtensions.AddOData(IMvcBuilder,Action{ODataOptions, IServiceProvider})" />
        /// </summary>
        /// <param name="options">OData options</param>
        /// <param name="provider">Service provider</param>
        public static void ConfigureVersioningOData<TMetadataController>(ODataOptions options, IServiceProvider provider)
            where TMetadataController : MetadataControllerBase
        {
            //NOTE:Replace metadata convension
            options.Conventions.Remove(options.Conventions.OfType<MetadataRoutingConvention>().First());
            options.Conventions.Add(new VersionedMetadataRoutingConvention<TMetadataController>());

            var apiVersionsProvider = provider.GetRequiredService<IApiVersionInfoProvider>();
            var modelProvider = provider.GetRequiredService<IODataModelProvider>();
            var versioningOptions = provider.GetRequiredService<IOptions<ODataVersioningOptions>>();
            var prefixTemplate = versioningOptions.Value.VersionPrefixTemplate;

            foreach (var version in apiVersionsProvider.Versions)
            {
                var prefix = VersioningRoutingPrefixHelper.GeneratePrefix(prefixTemplate, version).TrimStart('/');
                options.AddRouteComponents(prefix, modelProvider.GetEdmModel(version));
            }
        }
    }
}