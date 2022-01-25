using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IODataModelRequestProvider: IODataModelProvider
    {
        IEdmModel? GetRequestEdmModel(ApiVersion version);
    }
}