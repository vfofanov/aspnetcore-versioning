using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///     OData model provider for request
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class ODataModelProvider<TKey> : IODataModelRequestProvider
        where TKey : notnull
    {
        private readonly IEdmModelFactory<TKey> _edmFactory;
        private readonly ConcurrentDictionary<TKey, IEdmModel> _cached = new();

        protected ODataModelProvider(IEdmModelFactory<TKey> edmFactory)
        {
            _edmFactory = edmFactory;
        }

        public IEdmModel GetEdmModel(ApiVersionInfo versionInfo)
        {
            var model = GetEdmModel(versionInfo.Version, false);
            model.SetApiVersion(versionInfo);
            return model;
        }

        public IEdmModel GetRequestEdmModel(ApiVersion version)
        {
            return GetEdmModel(version, true);
        }

        private IEdmModel GetEdmModel(ApiVersion version, bool requestModel)
        {
            var key = _edmFactory.GetKey(version);
            return _cached.GetOrAdd(key, k => CreateModel(k, version, requestModel));
        }

        private IEdmModel CreateModel(TKey key, ApiVersion version, bool requestModel)
        {
            return _edmFactory.CreateModel(key, version, requestModel);
        }
    }
}