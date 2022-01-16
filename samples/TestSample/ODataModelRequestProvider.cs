#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stenn.AspNetCore.OData.Versioning;

namespace TestSample
{
    public class ODataModelRequestProvider : ODataModelRequestProviderBase<ApiVersion>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <inheritdoc />
        public ODataModelRequestProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <param name="version"></param>
        /// <inheritdoc />
        protected override ApiVersion GetKey(ApiVersion version)
        {
            return version;
        }

        /// <inheritdoc />
        protected override void FillEdmModel(EdmModelBuilder builder, ApiVersion key)
        {
            ODataModelProvider.FillModel(builder, key);
        }
    }
}