// Licensed under the MIT License.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace AspNetCore.OData.Versioning
{
    public interface IODataModelProvider
    {
        IEdmModel GetEdmModel(ApiVersion version, IServiceProvider provider);
    }
}
