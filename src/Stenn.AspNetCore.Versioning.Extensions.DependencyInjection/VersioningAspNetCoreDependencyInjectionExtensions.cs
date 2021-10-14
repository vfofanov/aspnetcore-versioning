using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Stenn.AspNetCore.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection extensions for register AspNet.Core versioning 
    /// </summary>
    public static class VersioningAspNetCoreDependencyInjectionExtensions
    {
        public static IServiceCollection AddVersioningForApi<TApiVersionProviderFactory, TVersioningRoutingPrefixProvider>(this IServiceCollection services)
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
            where TVersioningRoutingPrefixProvider : class, IVersioningRoutingPrefixProvider
        {
            AddVersioningForApiCore<TApiVersionProviderFactory>(services);
            services.TryAddSingleton<IVersioningRoutingPrefixProvider, TVersioningRoutingPrefixProvider>();

            return services;
        }
        
        /// <summary>
        /// Registerr services for api versioning
        /// </summary>
        /// <param name="services"></param>
        /// <param name="prefixFormatTemplate">version format template for <see cref="String.Format(string,object[])"/> wtih one {0} placeholder for <see cref="ApiVersion"/>version</param>
        /// <typeparam name="TApiVersionProviderFactory"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddVersioningForApi<TApiVersionProviderFactory>(this IServiceCollection services, string prefixFormatTemplate="{0}")
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
        {
            return services.AddVersioningForApi<TApiVersionProviderFactory>(_ => prefixFormatTemplate);
        }

        public static IServiceCollection AddVersioningForApi<TApiVersionProviderFactory>(this IServiceCollection services,
            Func<ControllerModel, string> getRoutePrefixFunc)
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
        {
            if (getRoutePrefixFunc == null)
            {
                throw new ArgumentNullException(nameof(getRoutePrefixFunc));
            }

            AddVersioningForApiCore<TApiVersionProviderFactory>(services);

            services.TryAddSingleton<IVersioningRoutingPrefixProvider>(_ => new DefaultVersioningRoutingPrefixProvider(getRoutePrefixFunc));

            return services;
        }

        public static void AddVersioningForApiCore<TApiVersionProviderFactory>(IServiceCollection services)
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
        {
            services.AddVersioningCore<TApiVersionProviderFactory>();

            //NOTE: Copies controller's models by versions and set versioned routing 
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ApiVersioningRoutingApplicationModelProvider>());
        }

        /// <summary>
        /// Add Api versioning core without model provider
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TApiVersionProviderFactory"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddVersioningCore<TApiVersionProviderFactory>(this IServiceCollection services)
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
        {
            services.TryAddSingleton<IApiVersionInfoProviderFactory, TApiVersionProviderFactory>();
            services.TryAddSingleton(provider => provider.GetRequiredService<IApiVersionInfoProviderFactory>().Create());
            
            return services;
        }
    }
}