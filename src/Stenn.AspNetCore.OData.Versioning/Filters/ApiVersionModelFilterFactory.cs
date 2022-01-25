using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public sealed class ApiVersionModelFilterFactory : IModelFilterFactory
    {
        private readonly ConcurrentDictionary<ApiVersion, ApiVersionModelFilter> _cached = new();

        public IModelFilter Create(ApiVersion version)
        {
            return _cached.GetOrAdd(version, v => new ApiVersionModelFilter(v));
        }
    }
}