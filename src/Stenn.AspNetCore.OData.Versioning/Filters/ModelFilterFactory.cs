using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class ModelFilterFactory<TFilter> : IModelFilterFactory
        where TFilter : IModelFilter, new()
    {
        /// <inheritdoc />
        IModelFilter IModelFilterFactory.Create(ApiVersion version)
        {
            return Create(version);
        }

        protected virtual TFilter Create(ApiVersion version)
        {
            return new TFilter();
        }
    }
}