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
        public static IServiceCollection AddVersioningForApi<TApiVersionProviderFactory>(this IServiceCollection services, string prefixFormatTemplate = "{0}")
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
        {
            services.TryAddSingleton<IVersioningRoutingPrefixProvider>(_ => new DefaultVersioningRoutingPrefixProvider(prefixFormatTemplate));

            return services.AddVersioningForApi<TApiVersionProviderFactory, DefaultVersioningRoutingPrefixProvider>();
        }

        public static IServiceCollection AddVersioningForApi<TApiVersionProviderFactory, TVersioningRoutingPrefixProvider>(this IServiceCollection services)
            where TApiVersionProviderFactory : class, IApiVersionInfoProviderFactory
        where TVersioningRoutingPrefixProvider:class,IVersioningRoutingPrefixProvider
        {
            services.AddVersioningCore<TApiVersionProviderFactory>();
            
            services.TryAddSingleton<IVersioningRoutingPrefixProvider, TVersioningRoutingPrefixProvider>();  
            
            //NOTE: Copies controller's models by versions and set versioned routing 
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ApiVersioningRoutingApplicationModelProvider>());

            return services;
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