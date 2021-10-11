using AspNetCore.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using TestSample.Controllers;

namespace TestSample
{
    public sealed class VersioningRoutingPrefixProvider : IVersioningRoutingPrefixProvider
    {
        /// <inheritdoc />
        public string GetPrefix(ControllerModel controller, ApiVersionInfo version)
        {
            if(controller.ControllerType.IsAssignableTo(typeof(BackOfficeController)))
            {
                return VersioningRoutingPrefixHelper.GeneratePrefix("api/backOffice/{0}", version);
            }
            return VersioningRoutingPrefixHelper.GeneratePrefix("api/{0}", version);
        }
    }
}