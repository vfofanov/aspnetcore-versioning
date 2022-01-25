using System.Collections.Generic;

namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    internal interface IODataActionParametersMapper
    {
        object Map(Dictionary<string, object> parameters);
    }
}