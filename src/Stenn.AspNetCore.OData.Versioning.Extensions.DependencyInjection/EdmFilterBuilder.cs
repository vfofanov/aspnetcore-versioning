using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    /// Builder for add <see cref="IEdmModelFilterFactory"/> to customize OData <see cref="IEdmModel"/> model
    /// </summary>
    public sealed class EdmModelFilterBuilder
    {
        private readonly IServiceCollection _services;

        public EdmModelFilterBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public void Clear()
        {
            _services.RemoveAll<IEdmModelFilterFactory>();
        }

        /// <summary>
        /// Add edm filter 
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        public EdmModelFilterBuilder Add<TFilter>() 
            where TFilter : IEdmModelFilter, new()
        {
            return AddFactory<EdmModelFilterFactory<TFilter>>();
        }
        
        /// <summary>
        /// Add edm filter 
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        public EdmModelFilterBuilder AddWithDefaultModelKey<TFilter>() 
            where TFilter : DefaultModelKeyEdmModelFilter, new()
        {
            return AddFactory<DefaultModelKeyEdmModelFilterFactory<TFilter>>();
        }

        /// <summary>
        /// Add edm filter factory as singleton 
        /// </summary>
        /// <typeparam name="TFactory"></typeparam>
        public EdmModelFilterBuilder AddFactory<TFactory>()
            where TFactory : class, IEdmModelFilterFactory
        {
            _services.TryAddEnumerable(ServiceDescriptor.Singleton<IEdmModelFilterFactory, TFactory>());
            return this;
        }
    }
}