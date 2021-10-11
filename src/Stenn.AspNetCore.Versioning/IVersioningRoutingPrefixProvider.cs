using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCore.Versioning
{
    public interface IVersioningRoutingPrefixProvider
    {
        string GetPrefix(ControllerModel controller, ApiVersionInfo version);

        
    }
}