using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    /// Builder for add <see cref="IModelFilterFactory"/> to customize OData <see cref="IEdmModel"/> model
    /// </summary>
    public sealed class ModelFilterBuilder
    {
        private readonly IServiceCollection _services;

        public ModelFilterBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public void Clear()
        {
            _services.RemoveAll<IModelFilterFactory>();
        }

        /// <summary>
        /// Add model filter 
        /// </summary>
        /// <param name="requestOnly">Is this filter applies per request or for convention model</param>
        /// <typeparam name="TFilter"></typeparam>
        /// <returns></returns>
        public ModelFilterBuilder Add<TFilter>(bool requestOnly) 
            where TFilter : IModelFilter, new()
        {
            return AddFactory<ModelFilterFactory<TFilter>>(requestOnly);
        }
        
        /// <summary>
        /// Add edm filter 
        /// </summary>
        /// <param name="requestOnly">Is this filter applies per request or for convention model</param>
        /// <typeparam name="TFilter"></typeparam>
        public ModelFilterBuilder AddWithDefaultModelKey<TFilter>(bool requestOnly) 
            where TFilter : DefaultModelKeyModelFilter, new()
        {
            return AddFactory<DefaultModelKeyModelFilterFactory<TFilter>>(requestOnly);
        }

        /// <summary>
        /// Add edm filter factory as singleton 
        /// </summary>
        /// <param name="requestOnly">Is this filter applies per request or for convention model</param>
        /// <typeparam name="TFactory"></typeparam>
        public ModelFilterBuilder AddFactory<TFactory>(bool requestOnly)
            where TFactory : class, IModelFilterFactory
        {
            _services.AddSingleton<TFactory>();
            _services.TryAddEnumerable(ServiceDescriptor.Singleton<IModelFilterFactoryRegistration, ModelFilterFactoryRegistration<TFactory>>(
                provider => new ModelFilterFactoryRegistration<TFactory>(requestOnly, provider.GetRequiredService<TFactory>())));
            return this;
        }
    }
}