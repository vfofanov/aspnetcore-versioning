using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning.Extensions.DependencyInjection
{
    /// <summary>
    /// Builder for add <see cref="IEdmModelFilterFactory"/> to customize OData <see cref="IEdmModel"/> model
    /// </summary>
    public sealed class EdmFilterBuilder
    {
        private readonly IServiceCollection _services;

        public EdmFilterBuilder(IServiceCollection services)
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
        public EdmFilterBuilder Add<TFilter>() 
            where TFilter : IEdmModelFilter, new()
        {
            return AddFactory<EdmModelFilterFactory<TFilter>>();
        }

        /// <summary>
        /// Add edm filter factory as singleton 
        /// </summary>
        /// <typeparam name="TFactory"></typeparam>
        public EdmFilterBuilder AddFactory<TFactory>()
            where TFactory : class, IEdmModelFilterFactory
        {
            _services.TryAddEnumerable(ServiceDescriptor.Singleton<IEdmModelFilterFactory, TFactory>());
            return this;
        }
    }
}