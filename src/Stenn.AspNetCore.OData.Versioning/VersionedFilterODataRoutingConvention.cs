using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.OData.Routing.Template;
using OData8VersioningPrototype.ApiConventions;

namespace OData8VersioningPrototype.ODataConfigurations.Common
{
    /// <summary>
    /// Versioning cleanup after name conventions
    /// </summary>
    public sealed class VersionedFilterODataRoutingConvention: IODataControllerActionConvention
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="versionInfoProvider"></param>
        public VersionedFilterODataRoutingConvention(IApiVersionInfoProvider versionInfoProvider)
        {
            _versionInfoProvider = versionInfoProvider ?? throw new ArgumentNullException(nameof(versionInfoProvider));
        }

        /// <summary>
        /// Gets the order value for determining the order of execution of conventions.
        /// After all.
        /// </summary>
        public int Order => 2000;

        /// <inheritdoc />
        public bool AppliesToController(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }
            var version = context.GetODataApiVersion();
            var controller = context.Controller;

            if (!IsApiVersionMatch(controller.Attributes, version))
            {
                controller.Selectors.Clear();
                controller.Actions.Clear();
            }
            else
            {
                ProcessSelectors(controller.Selectors, version);
                foreach (var action in controller.Actions)
                {
                    ProcessSelectors(action.Selectors, version);
                }
                
            }
            return false;
        }

        private void ProcessSelectors(IList<SelectorModel> selectors, ApiVersion version)
        {
            for (var i = 0; i < selectors.Count; i++)
            {
                var selector = selectors[i];
                if (!IsApiVersionMatch(selector.EndpointMetadata, version))
                {
                    selectors.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <inheritdoc />
        public bool AppliesToAction(ODataControllerActionContext context)
        {
            return true;
        }

        private static bool IsApiVersionMatch(IEnumerable<object> attributes, ApiVersion version)
        {
            var result = true;
            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case IApiVersionNeutral:
                        return true;
                    case IApiVersionProvider versionProvider:
                    {
                        for (var v = 0; v < versionProvider.Versions.Count; v++)
                        {
                            if (versionProvider.Versions[v] == version)
                            {
                                return true;
                            }
                        }
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
    }
}