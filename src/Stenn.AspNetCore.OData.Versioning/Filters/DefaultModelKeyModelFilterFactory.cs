using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public sealed class DefaultModelKeyModelFilterFactory<TFilter> : IModelFilterFactory
        where TFilter : DefaultModelKeyModelFilter, new()
    {
        private readonly TFilter _filter;

        public DefaultModelKeyModelFilterFactory()
        {
            _filter = new TFilter();
        }

        /// <inheritdoc />
        public IModelFilter Create(ApiVersion version)
        {
            return _filter;
        }
    }
}