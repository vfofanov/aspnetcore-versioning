using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public sealed class VersioningEdmModelFilterFactory<TFilter> : EdmModelFilterFactory<TFilter>
        where TFilter : IVersioningEdmModelFilter, new()
    {
        protected override TFilter Create(ApiVersion version)
        {
            var filter = base.Create(version);
            filter.SetVersion(version);
            return filter;
        }
    }
}