using System;
using Microsoft.AspNetCore.Mvc;
using Stenn.AspNetCore.OData.Versioning;

namespace TestSample
{
    public class ODataModelRequestProvider : ODataModelRequestProviderBase<ApiVersion>
    {
        /// <inheritdoc />
        protected override ApiVersion GetKey(ApiVersion version, IServiceProvider provider)
        {
            return version;
        }

        /// <inheritdoc />
        protected override void FillEdmModel(AdvODataConventionModelBuilder builder, ApiVersion key)
        {
            ODataModelProvider.FillModel(builder, key);
        }
    }
}