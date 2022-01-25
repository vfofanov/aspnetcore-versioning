#nullable enable

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
        /// <param name="requestModel"></param>
        IEdmModel CreateModel(EdmModelBuilder builder, bool requestModel);
    }
}