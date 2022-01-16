using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

#nullable enable
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class EdmModelFilterFactory<TFilter> : IEdmModelFilterFactory
        where TFilter : IEdmModelFilter
    {
        private readonly IServiceProvider _provider;

        public EdmModelFilterFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        IEdmModelFilter IEdmModelFilterFactory.Create(ApiVersion version)
        {
            return Create(version);
        }

        protected virtual TFilter Create(ApiVersion version)
        {
            return _provider.GetRequiredService<TFilter>();
        }
    }

    public sealed class VersioningEdmModelFilterFactory<TFilter> : EdmModelFilterFactory<TFilter>
        where TFilter : IVersioningEdmModelFilter
    {
        public VersioningEdmModelFilterFactory(IServiceProvider provider) 
            : base(provider)
        {
        }

        protected override TFilter Create(ApiVersion version)
        {
            var filter = base.Create(version);
            filter.SetVersion(version);
            return filter;
        }
    }
}