// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace AspNetCore.OData.Versioning
{
    public abstract class ODataModelProvider<TKey> : IODataModelProvider, IODataConventionModelProvider
    {

        private readonly ConcurrentDictionary<TKey, IEdmModel> _cached = new();

        public IEdmModel GetNameConventionEdmModel(ApiVersion apiVersion)
        {
            var key = GetNameConventionKey(apiVersion);
            return _cached.GetOrAdd(key, k => CreateNameConventionModel(apiVersion, k));
        }

        public IEdmModel GetEdmModel(ApiVersion apiVersion, IServiceProvider serviceProvider)
        {
            var key = GetKey(apiVersion, serviceProvider);
            return _cached.GetOrAdd(key, CreateModel);
        }

        protected AdvODataConventionModelBuilder CreateBuilder(ODataConventionModelBuilder builder = null)
        {
            return new AdvODataConventionModelBuilder(builder ?? new ODataConventionModelBuilder());
        }

        protected abstract TKey GetNameConventionKey(ApiVersion apiVersion);
        protected abstract TKey GetKey(ApiVersion version, IServiceProvider provider);
        protected abstract void FillEdmModel(AdvODataConventionModelBuilder builder, TKey key);

        private IEdmModel CreateNameConventionModel(ApiVersion apiVersion, TKey key)
        {
            var model = CreateModel(key);

            model.SetApiVersion(apiVersion);

            return model;
        }

        private IEdmModel CreateModel(TKey key)
        {
            var builder = CreateBuilder();
            
            FillEdmModel(builder, key);
            
            return builder.GetEdmModel();
        }
    }
}
