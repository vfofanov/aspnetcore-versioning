using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public sealed class ApiVersionEdmModelFilterFactory : IEdmModelFilterFactory
    {
        private readonly ConcurrentDictionary<ApiVersion, ApiVersionEdmModelFilter> _cached = new();

        public IEdmModelFilter Create(ApiVersion version)
        {
            return _cached.GetOrAdd(version, v => new ApiVersionEdmModelFilter(v));
        }
    }
}