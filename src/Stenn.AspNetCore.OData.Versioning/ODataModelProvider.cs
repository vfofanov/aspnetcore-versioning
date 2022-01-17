using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.OData.Versioning.Filters;
using Stenn.AspNetCore.OData.Versioning.Operations;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///     OData model provider for request
    /// </summary>
    public sealed class ODataModelProvider : IODataModelRequestProvider
    {
        private readonly IEdmModelFactory _edmFactory;
        private readonly ConcurrentDictionary<EdmModelKey, IEdmModel> _cached = new();

        public ODataModelProvider(IEdmModelFactory edmFactory, 
            IEdmModelMutatorFactory mutatorFactory, 
            IEdmModelOperationExtractorFactory operationExtractorFactory)
        {
            _edmFactory = edmFactory;
            MutatorFactory = mutatorFactory;
            OperationExtractorFactory = operationExtractorFactory;
        }

        private IEdmModelMutatorFactory MutatorFactory { get; }
        private IEdmModelOperationExtractorFactory OperationExtractorFactory { get; }
        
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
            var builder = _edmFactory.CreateBuilder();
            builder.Mutator = MutatorFactory.Create(builder.Builder, version, requestModel);
            builder.OperationExtractor = OperationExtractorFactory.Create(builder);
            
            var key = builder.Mutator.GetKey();

            return _cached.GetOrAdd(key, _ => _edmFactory.CreateModel(builder, requestModel));
        }
    }
}