﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
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
        ///     Initialize custom <see cref="IModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningODataModelPerRequest<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction,
            Action<ModelFilterBuilder> initFilters) where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory
        {
            return AddVersioningOData<TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, true, initFilters);
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
        ///     Initialize custom <see cref="IModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction,
            Action<ModelFilterBuilder>? initFilters = null)
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            return AddVersioningOData<TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, false, initFilters);
        }


        /// <summary>
        ///     Adds essential versioning OData services to the specified <see cref="IMvcBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add services to.</param>
        /// <param name="versioningSetupAction">
        ///     The OData versioning options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <param name="setupAction">
        ///     The OData options to configure the services with,
        ///     including access to a service provider which you can resolve services from.
        /// </param>
        /// <param name="modelPerRequest">Is edm model will be generated for each request</param>
        /// <param name="initFilters">
        ///     Initialize custom <see cref="IModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// ///
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions> versioningSetupAction,
            Action<ODataOptions> setupAction,
            bool modelPerRequest,
            Action<ModelFilterBuilder>? initFilters = null) 
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory
        {
            return builder.AddVersioningOData<TMetadataController, TEdmFactory>(
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
        ///     Initialize custom <see cref="IModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningODataModelPerRequest<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction,
            Action<ModelFilterBuilder>? initFilters = null) where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory
        {
            return AddVersioningOData<TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, initFilters, true);
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
        ///     Initialize custom <see cref="IModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        public static IMvcBuilder AddVersioningOData<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction,
            Action<ModelFilterBuilder>? initFilters = null)
            where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory
        {
            return AddVersioningOData<TMetadataController, TEdmFactory>(builder, versioningSetupAction, setupAction, initFilters, false);
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
        ///     Initialize custom <see cref="IModelFilter" /> filters for change <see cref="IEdmModel" />
        ///     model
        /// </param>
        /// <param name="modelPerRequest"></param>
        /// <returns>A <see cref="IMvcBuilder" /> that can be used to further configure the OData services.</returns>
        private static IMvcBuilder AddVersioningOData<TMetadataController, TEdmFactory>(this IMvcBuilder builder,
            Action<ODataVersioningOptions, IServiceProvider> versioningSetupAction,
            Action<ODataOptions, IServiceProvider> setupAction,
            Action<ModelFilterBuilder>? initFilters,
            bool modelPerRequest) where TMetadataController : MetadataControllerBase
            where TEdmFactory : class, IEdmModelFactory
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddVersioningODataCore<TEdmFactory>(initFilters, modelPerRequest);
            builder.Services.AddOptions<ODataVersioningOptions>().Configure(versioningSetupAction);

            builder.AddOData((options, provider) =>
            {
                ConfigureVersioningOData<TMetadataController>(options, provider);
                setupAction(options, provider);
            });

            return builder;
        }

        public static IServiceCollection AddVersioningODataCore<TEdmFactory>(this IServiceCollection services, 
            Action<ModelFilterBuilder>? initFilters,
            bool modelPerRequest)
            where TEdmFactory : class, IEdmModelFactory
        {
            services.TryAddSingleton<IEdmModelMutatorFactory, EdmModelMutatorFactory>();
            services.TryAddSingleton<IEdmModelOperationExtractorFactory, EdmModelOperationExtractorFactory>();
            services.TryAddSingleton<IEdmModelFactory, TEdmFactory>();

            services.TryAddSingleton<ODataModelProvider>();
            services.TryAddSingleton<IODataModelProvider>(p => p.GetRequiredService<ODataModelProvider>());
            services.TryAddSingleton<IODataModelRequestProvider>(p => p.GetRequiredService<ODataModelProvider>());

            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ODataVersioningRoutingApplicationModelProvider>());

            //Options
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ODataVersioningOptions>, ODataVersioningOptionsSetup>());

            if (modelPerRequest)
            {
                //NOTE: Change OData matcher policy for apply model per request
                services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, VersioningODataRoutingMatcherPolicy>());
            }

            var filtersBuilder = new ModelFilterBuilder(services);
            filtersBuilder.AddApiVersioning();
            filtersBuilder.AddTextJsonIgnore();
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
        
        /// <summary>
        ///     Adds supports for ignore model parts marked with attribute inherited from <see cref="IApiVersionProvider"/>. eg. <see cref="ApiVersionAttribute" />
        /// </summary>
        /// <param name="builder">The <see cref="ModelFilterBuilder" /> to add filters to.</param>
        /// <returns>A <see cref="ModelFilterBuilder" /> that can be used to further configure the filters.</returns>
        public static ModelFilterBuilder AddApiVersioning(this ModelFilterBuilder builder)
        {
            return builder.AddFactory<ApiVersionModelFilterFactory>(false);
        }
        
        /// <summary>
        ///     Adds supports for ignore properties marked with <see cref="JsonIgnoreAttribute" />
        /// </summary>
        /// <param name="builder">The <see cref="ModelFilterBuilder" /> to add filters to.</param>
        /// <returns>A <see cref="ModelFilterBuilder" /> that can be used to further configure the filters.</returns>
        public static ModelFilterBuilder AddTextJsonIgnore(this ModelFilterBuilder builder)
        {
            return builder.AddWithDefaultModelKey<TextJsonIgnoreAttributeModelFilter>(false);
        }
    }
}