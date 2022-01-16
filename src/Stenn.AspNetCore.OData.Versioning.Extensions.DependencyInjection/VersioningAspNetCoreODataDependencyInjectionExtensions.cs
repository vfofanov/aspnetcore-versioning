using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.OData.Versioning.Filters;
using Stenn.AspNetCore.OData.Versioning.Operations;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for registee AspNet.Core OData versioning
    /// </summary>
    public static class VersioningAspNetCoreODataDependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />. OData edm model will be
        ///     created and cached per request
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
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IEdmModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningODataModelPerRequest<TModelKey, TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction,
            Action<EdmFilterBuilder> initFilters)
            where TModelKey : notnull
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory<TModelKey>
        {
            return AddVersioningOData<TModelKey, TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, initFilters, true);
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
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IEdmModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction,
            Action<EdmFilterBuilder>? initFilters = null)
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory<ApiVersion>
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            return AddVersioningOData<ApiVersion, TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, initFilters, false);
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
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IEdmModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <param name="modelPerRequest">Is edm model will be generated for each request</param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TModelKey, TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction,
            Action<EdmFilterBuilder>? initFilters = null,
            bool modelPerRequest = false)
            where TModelKey : notnull
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory<TModelKey>
        {
            return builder.AddVersioningOData<TModelKey, TMetadataController, TEdmFactory>(
                (options, _) => versioningSetupAction(options),
                (options, _) => setupAction(options),
                initFilters,
                modelPerRequest);
        }

        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />. OData edm model will be
        ///     created and cached per request
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
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IEdmModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningODataModelPerRequest<TModelKey, TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction,
            Action<EdmFilterBuilder>? initFilters = null)
            where TModelKey : notnull
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory<TModelKey>
        {
            return AddVersioningOData<TModelKey, TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, initFilters, true);
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
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IEdmModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction,
            Action<EdmFilterBuilder>? initFilters = null)
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory<ApiVersion>
        {
            return AddVersioningOData<ApiVersion, TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, initFilters, false);
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
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IEdmModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <param name="modelPerRequest"></param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        private static IMvcBuilder AddVersioningOData<TModelKey, TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction,
            Action<EdmFilterBuilder>? initFilters,
            bool modelPerRequest)
            where TModelKey : notnull
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory<TModelKey>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddVersioningODataCore<TModelKey, TEdmFactory>(initFilters, modelPerRequest);
            builder.Services.AddOptions<ODataVersioningOptions>().Configure(versioningSetupAction);

            builder.AddOData((options, provider) =>
            {
                ConfigureVersioningOData<TMetadataController>(options, provider);
                setupAction(options, provider);
            });

            return builder;
        }

        public static IServiceCollection AddVersioningODataCore<TModelKey, TEdmFactory>(this IServiceCollection services, Action<EdmFilterBuilder>? initFilters,
            bool modelPerRequest)
            where TEdmFactory : class, IEdmModelFactory<TModelKey>
            where TModelKey : notnull
        {
            services.TryAddSingleton<IEdmModelMutatorFactory, EdmModelMutatorFactory>();
            services.TryAddSingleton<IEdmModelOperationExtractorFactory, EdmModelOperationExtractorFactory>();
            services.TryAddSingleton<IEdmModelFactory<TModelKey>, TEdmFactory>();

            services.TryAddSingleton<ODataModelProvider<TModelKey>>();
            services.TryAddSingleton<IODataModelProvider>(p => p.GetRequiredService<ODataModelProvider<TModelKey>>());
            services.TryAddSingleton<IODataModelRequestProvider>(p => p.GetRequiredService<ODataModelProvider<TModelKey>>());

            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ODataVersioningRoutingApplicationModelProvider>());

            //Options
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ODataVersioningOptions>, ODataVersioningOptionsSetup>());

            //For ApiExplorer
            services.TryAddSingleton<IEdmModelSelector, EdmModelSelector>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, ODataQueryParametersApiDescriptionProvider>());

            if (modelPerRequest)
            {
                //NOTE: Change OData matcher policy for apply model per request
                services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, VersioningODataRoutingMatcherPolicy>());
            }

            var filtersBuilder = new EdmFilterBuilder(services);
            filtersBuilder.AddFactory<VersioningEdmModelFilterFactory<ApiVersionEdmModelFilter>>();
            filtersBuilder.Add<TextJsonIgnoreAttributeEdmModelFilter>();
            initFilters?.Invoke(filtersBuilder);

            return services;
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