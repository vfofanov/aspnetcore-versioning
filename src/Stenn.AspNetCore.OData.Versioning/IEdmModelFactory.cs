#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IEdmModelFactory
    {
        EdmModelBuilder CreateBuilder();

        /// <summary>
        /// Create edm model
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="version"></param>
        /// <param name="requestModel"></param>
        IEdmModel CreateModel(EdmModelBuilder builder, ApiVersion version, bool requestModel);
    }
}