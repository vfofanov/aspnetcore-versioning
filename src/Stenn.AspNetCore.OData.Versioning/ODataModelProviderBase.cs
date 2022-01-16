// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning
{
    public abstract class ODataModelProviderBase : ODataModelProviderBase<EdmModelBuilder>
    {
        protected override EdmModelBuilder CreateBuilder()
        {
            return new EdmModelBuilder(new ODataConventionModelBuilder());
        }
    }

    public abstract class ODataModelProviderBase<TModelBuilder> : IODataModelProvider
        where TModelBuilder : IEdmModelBuilder
    {
        public IEdmModel GetEdmModel(ApiVersionInfo versionInfo)
        {
            var builder = CreateBuilder();

            FillEdmModel(builder, versionInfo.Version);
            var model = builder.GetEdmModel();

            model.SetApiVersion(versionInfo);

            return model;
        }

        protected abstract TModelBuilder CreateBuilder();

        protected abstract void FillEdmModel(TModelBuilder builder, ApiVersion version);
    }
}