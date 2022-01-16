using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public sealed class DefaultModelKeyEdmModelFilterFactory<TFilter> : IEdmModelFilterFactory
        where TFilter : DefaultModelKeyEdmModelFilter, new()
    {
        private readonly TFilter _filter;

        public DefaultModelKeyEdmModelFilterFactory()
        {
            _filter = new TFilter();
        }

        /// <inheritdoc />
        public IEdmModelFilter Create(ApiVersion version)
        {
            return _filter;
        }
    }
}