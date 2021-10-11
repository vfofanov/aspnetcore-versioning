using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Stenn.AspNetCore.Versioning
{
    public interface IVersioningRoutingPrefixProvider
    {
        string GetPrefix(ControllerModel controller, ApiVersionInfo version);

        
    }
}