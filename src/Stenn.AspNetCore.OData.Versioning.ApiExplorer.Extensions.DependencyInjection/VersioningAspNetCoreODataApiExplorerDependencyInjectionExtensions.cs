using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.OData.Versioning.ApiExplorer;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for registee AspNet.Core OData versioning Api Explorer
    /// </summary>
    public static class VersioningAspNetCoreODataApiExplorerDependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds services for make OData visible for <see cref="IApiExplorerModel" />. Used for open-api generation
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddVersioningODataApiExplorer(this IServiceCollection services,
            Action<ODataVersioningOptions>? optionsAction = null)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<ODataVersioningApiExplorerOptions>, ODataVersioningApiExplorerOptionsSetup>());
            if (optionsAction != null)
            {
                services.AddOptions<ODataVersioningOptions>().Configure(optionsAction);
            }

            services.TryAddSingleton<IEdmModelSelector, EdmModelSelector>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, ODataQueryParametersApiDescriptionProvider>());

            return services;
        }
    }
}