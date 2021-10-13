using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IODataModelProvider
    {
        /// <summary>
        ///     Model for run standard entites' name conventions for odata controllers.
        /// </summary>
        /// <returns></returns>
        IEdmModel GetEdmModel(ApiVersion version);
    }
}