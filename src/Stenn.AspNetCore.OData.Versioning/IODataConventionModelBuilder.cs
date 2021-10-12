using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IODataConventionModelBuilder
    {
        IEdmModel GetEdmModel();
    }
}