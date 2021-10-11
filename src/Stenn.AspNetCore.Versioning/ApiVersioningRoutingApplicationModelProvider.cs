using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Versioning routing provider for controllers with attribute <see cref="ApiControllerAttribute"/>
    /// </summary>
    public sealed class ApiVersioningRoutingApplicationModelProvider : VersioningRoutingApplicationModelProvider
    {
        /// <inheritdoc />
        public ApiVersioningRoutingApplicationModelProvider(IApiVersionInfoProvider versionInfoProvider, IVersioningRoutingPrefixProvider prefixProvider) 
            : base(versionInfoProvider, prefixProvider)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<ControllerModel> GetControllers(ApplicationModelProviderContext context)
        {
            return context.Result.Controllers.Where(c => c.Attributes.OfType<ApiControllerAttribute>().Any());
        }
    }
}