#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ApiConventions
{
    public class ApiVersioningODataRoutingApplicationModelProvider : IApplicationModelProvider
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="versionInfoProvider"></param>
        public ApiVersioningODataRoutingApplicationModelProvider(IApiVersionInfoProvider versionInfoProvider)
        {
            _versionInfoProvider = versionInfoProvider ?? throw new ArgumentNullException(nameof(versionInfoProvider));
        }

        //After all providers
        /// <inheritdoc />
        public int Order => 2000;

        /// <inheritdoc />
        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            var apiControllers = context.Result.Controllers.Where(c => c.Attributes.OfType<ODataAttributeRoutingAttribute>().Any()).ToList();

            for (var i = 0; i < apiControllers.Count; i++)
            {
                var controller = apiControllers[i];
                var version = controller.GetODataApiVersion();

                if (version == null || !IsApiVersionMatch(controller.Attributes, version))
                {
                    context.Result.Controllers.Remove(controller);
                }
                ProcessController(controller, _versionInfoProvider.Versions.First(v => v == version));
            }
        }

        private void ProcessController(ControllerModel controller, ApiVersionInfo versionInfo)
        {
            controller.SetProperty(versionInfo.Annotation);

            controller.ApiExplorer.GroupName = versionInfo.PathPartName;
            controller.ApiExplorer.IsVisible = true;
                
            for (var i = 0; i < controller.Actions.Count; i++)
            {
                var action = controller.Actions[i];
                
                action.ApiExplorer.GroupName = versionInfo.PathPartName;
                action.ApiExplorer.IsVisible = true;
                
                if (IsApiVersionMatch(action.Attributes, versionInfo.Version))
                {
                    action.SetProperty(versionInfo.Annotation);
                }
                else
                {
                    controller.Actions.RemoveAt(i);
                    i--;
                }
            }
        }

        public IReadOnlyList<ApiVersionInfo> GetVersions(IReadOnlyList<object> attributes)
        {
            if (IsApiVersionNeutral(attributes))
            {
                return _versionInfoProvider.Versions;
            }

            var result = new List<ApiVersionInfo>(_versionInfoProvider.Versions.Count);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (attribute is not IApiVersionProvider versionProvider)
                {
                    continue;
                }

                foreach (var version in versionProvider.Versions)
                {
                    if (result.FindIndex(x => x == version) >= 0)
                    {
                        continue;
                    }
                    result.Add(_versionInfoProvider.Versions.First(x => x == version));
                }
            }
            return result;
        }

        private static bool IsApiVersionNeutral(IReadOnlyList<object> attributes)
        {
            var result = true;
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (attribute is IApiVersionNeutral)
                {
                    return true;
                }
                if (attribute is IApiVersionInfoProvider)
                {
                    result = false;
                }
            }
            return result;
        }

        private static bool IsApiVersionMatch(IReadOnlyList<object> attributes, ApiVersion version)
        {
            var result = true;
            for (var i = 0; i < attributes.Count; i++)
            {
                switch (attributes[i])
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

        /// <inheritdoc />
        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }
    }
}
