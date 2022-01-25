using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Stenn.AspNetCore.OData.Versioning
{
    public abstract class ODataController<TEntity> : ODataController, IODataController<TEntity>
    {
    }
}