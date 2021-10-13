// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public abstract class ODataModelProviderBase : ODataModelProviderBase<AdvODataConventionModelBuilder>
    {
        protected override AdvODataConventionModelBuilder CreateBuilder()
        {
            return new AdvODataConventionModelBuilder(new ODataConventionModelBuilder());
        }
    }

    public abstract class ODataModelProviderBase<TModelBuilder> : IODataModelProvider
        where TModelBuilder : IODataConventionModelBuilder
    {
        public IEdmModel GetEdmModel(ApiVersion version)
        {
            var builder = CreateBuilder();

            FillEdmModel(builder, version);
            var model = builder.GetEdmModel();

            model.SetApiVersion(version);

            return model;
        }

        protected abstract TModelBuilder CreateBuilder();

        protected abstract void FillEdmModel(TModelBuilder builder, ApiVersion version);
    }
}