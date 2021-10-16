using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.Extensions.Options;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///     Versioning routing provider for odata controllers with attribute <see cref="ODataAttributeRoutingAttribute" />
    /// </summary>
    public class ODataVersioningRoutingApplicationModelProvider : VersioningRoutingApplicationModelProvider
    {
        private readonly IOptions<ODataVersioningOptions> _options;

        /// <inheritdoc />
        public ODataVersioningRoutingApplicationModelProvider(IApiVersionInfoProvider versionInfoProvider, IOptions<ODataVersioningOptions> options)
            : base(versionInfoProvider, new DefaultVersioningRoutingPrefixProvider(_ => options.Value.VersionPrefixTemplate))
        {
            _options = options;
        }

        protected ODataVersioningOptions Options => _options.Value;
        
        /// <inheritdoc />
        protected override IEnumerable<ControllerModel> GetControllers(ApplicationModelProviderContext context)
        {
            return context.Result.Controllers.Where(c => c.ControllerType.IsAssignableTo(typeof(MetadataControllerBase)) ||
                                                         c.Attributes.OfType<ODataAttributeRoutingAttribute>().Any());
        }

        /// <inheritdoc />
        protected override void CleanUpActionSelectors(string prefix, ApiVersionInfo version, IList<SelectorModel> selectors)
        {
            //NOTE: After OData model provider need clean up selectors by version 
            CleanUpSelectors(prefix, version, selectors);

            if (!Options.RouteOptions.EnableEntitySetCount)
            {
                var entitySetCountSelectors = selectors
                    .Where(s => s.EndpointMetadata.OfType<ODataRoutingMetadata>().Any(m => m.Template.OfType<CountSegmentTemplate>().Any())).ToList();
                foreach (var selector in entitySetCountSelectors)
                {
                    selectors.Remove(selector);
                }    
            }
        }
    }
}