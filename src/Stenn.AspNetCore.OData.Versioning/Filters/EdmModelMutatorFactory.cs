#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class EdmModelMutatorFactory : IEdmModelMutatorFactory
    {
        private readonly IModelFilterFactoryRegistration[] _edmFilterFactories;
        public EdmModelMutatorFactory(IEnumerable<IModelFilterFactoryRegistration> edmFilterFactories)
        {
            _edmFilterFactories = edmFilterFactories.ToArray();
        }
        protected virtual IEdmModelMutator Create(ODataModelBuilder builder, bool requestModel, IEnumerable<IModelFilter> edmFilters)
        {
            return new EdmModelMutator(builder,requestModel, edmFilters);
        }
        public IEdmModelMutator Create(ODataModelBuilder builder, ApiVersion version, bool requestModel)
        {
            IEnumerable<IModelFilterFactoryRegistration> edmFilterFactories = _edmFilterFactories;
            if (!requestModel)
            {
                edmFilterFactories = edmFilterFactories.Where(f => !f.RequestOnly);
            }
            var edmFilters = edmFilterFactories.Select(f => f.Factory.Create(version));
            
            var context = Create(builder, requestModel, edmFilters);
            return context;
        }
    }
}