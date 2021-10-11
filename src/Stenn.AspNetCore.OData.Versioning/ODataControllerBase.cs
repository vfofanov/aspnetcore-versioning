using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace AspNetCore.OData.Versioning
{
    public abstract class ODataController<TEntity> : ODataController, IODataController<TEntity>
    {
    }
}