using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IODataModelProvider
    {
        /// <summary>
        ///     Model for run standard entites' name conventions for odata controllers.
        /// </summary>
        /// <returns></returns>
        IEdmModel GetEdmModel(ApiVersionInfo version);
    }
}