#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class EdmModelMutatorFactory : IEdmModelMutatorFactory
    {
        private readonly IEdmModelFilterFactory[] _edmFilterFactories;
        
        /// <summary>
        /// </summary>
        public EdmModelMutatorFactory(IEnumerable<IEdmModelFilterFactory> edmFilterFactories)
        {
            _edmFilterFactories = edmFilterFactories.ToArray();
        }
        protected virtual IEdmModelMutator Create(ODataModelBuilder builder, bool requestModel, IEnumerable<IEdmModelFilter> edmFilters)
        {
            return new EdmModelMutator(builder,requestModel, edmFilters);
        }
        public IEdmModelMutator Create(ODataModelBuilder builder, ApiVersion version, bool requestModel)
        {
            var edmFilters = _edmFilterFactories.Select(f => f.Create(version));
            if (!requestModel)
            {
                edmFilters = edmFilters.Where(f => !f.ForRequestModelOnly);
            }
            var context = Create(builder, requestModel, edmFilters);
            return context;
        }
    }
}