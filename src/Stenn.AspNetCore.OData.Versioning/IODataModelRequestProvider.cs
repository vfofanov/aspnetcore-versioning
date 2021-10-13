using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IODataModelRequestProvider
    {
        IEdmModel? GetRequestEdmModel(ApiVersion version, IServiceProvider provider);
    }
}