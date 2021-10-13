using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public abstract class ODataModelRequestProviderBase<TKey> : ODataModelRequestProviderBase<TKey, AdvODataConventionModelBuilder>
        where TKey : notnull
    {
        protected override AdvODataConventionModelBuilder CreateBuilder()
        {
            return new AdvODataConventionModelBuilder(new ODataConventionModelBuilder());
        }
    }

    /// <summary>
    ///     OData model provider for request
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TModelBuilder"></typeparam>
    public abstract class ODataModelRequestProviderBase<TKey, TModelBuilder> : IODataModelRequestProvider
        where TKey : notnull
        where TModelBuilder : IODataConventionModelBuilder
    {
        private readonly ConcurrentDictionary<TKey, IEdmModel> _cached = new();

        public IEdmModel GetRequestEdmModel(ApiVersion apiVersion, IServiceProvider serviceProvider)
        {
            var key = GetKey(apiVersion, serviceProvider);
            return _cached.GetOrAdd(key, CreateModel);
        }

        protected abstract TModelBuilder CreateBuilder();

        protected abstract TKey GetKey(ApiVersion version, IServiceProvider provider);
        protected abstract void FillEdmModel(TModelBuilder builder, TKey key);

        private IEdmModel CreateModel(TKey key)
        {
            var builder = CreateBuilder();

            FillEdmModel(builder, key);

            return builder.GetEdmModel();
        }
    }
}