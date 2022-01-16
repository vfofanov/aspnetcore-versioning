using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class EdmModelFilterFactory<TFilter> : IEdmModelFilterFactory
        where TFilter : IEdmModelFilter, new()
    {
        /// <inheritdoc />
        IEdmModelFilter IEdmModelFilterFactory.Create(ApiVersion version)
        {
            return Create(version);
        }

        protected virtual TFilter Create(ApiVersion version)
        {
            return new TFilter();
        }
    }
}