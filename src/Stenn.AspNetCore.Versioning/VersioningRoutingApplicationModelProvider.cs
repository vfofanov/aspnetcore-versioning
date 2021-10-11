using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Stenn.AspNetCore.Versioning
{
    public abstract class VersioningRoutingApplicationModelProvider : IApplicationModelProvider
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;
        private readonly IVersioningRoutingPrefixProvider _prefixProvider;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="versionInfoProvider"></param>
        /// <param name="prefixProvider"></param>
        protected VersioningRoutingApplicationModelProvider(IApiVersionInfoProvider versionInfoProvider, IVersioningRoutingPrefixProvider prefixProvider)
        {
            _versionInfoProvider = versionInfoProvider ?? throw new ArgumentNullException(nameof(versionInfoProvider));
            _prefixProvider = prefixProvider ?? throw new ArgumentNullException(nameof(prefixProvider));
        }

        //After all providers
        /// <inheritdoc />
        public virtual int Order => -500;

        /// <inheritdoc />
        public virtual void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            var apiControllers = GetControllers(context).ToList();

            foreach (var controller in apiControllers)
            {
                var versions = GetVersions(controller.Attributes);
                for (var i = 1; i < versions.Count; i++)
                {
                    var version = versions[i];
                    var current = new ControllerModel(controller);
                    ProcessController(current, version);
                    context.Result.Controllers.Add(current);
                }
                ProcessController(controller, versions[0]);
            }
        }

        protected abstract IEnumerable<ControllerModel> GetControllers(ApplicationModelProviderContext context);

        private void ProcessController(ControllerModel controller, ApiVersionInfo version)
        {
            var prefix = _prefixProvider.GetPrefix(controller, version);
            
            controller.SetProperty(version.Annotation);
            ApplyRoutePrefix(controller, prefix);

            CleanUpControllerSelectors(prefix, version, controller.Selectors);

            controller.ApiExplorer.GroupName = version.RoutePathName;
            controller.ApiExplorer.IsVisible = true;
                
            for (var i = 0; i < controller.Actions.Count; i++)
            {
                var action = controller.Actions[i];
                
                action.ApiExplorer.GroupName = version.RoutePathName;
                action.ApiExplorer.IsVisible = true;
                
                if (IsApiVersionMatch(action.Attributes, version.Version))
                {
                    action.SetProperty(version.Annotation);
                    CleanUpActionSelectors(prefix, version, action.Selectors);
                }
                else
                {
                    controller.Actions.RemoveAt(i);
                    i--;
                }
            }
        }


        protected virtual void CleanUpControllerSelectors(string prefix, ApiVersionInfo version, IList<SelectorModel> selectors)
        {
            CleanUpSelectors(prefix, version, selectors);
        }

        protected virtual void CleanUpActionSelectors(string prefix, ApiVersionInfo version, IList<SelectorModel> selectors)
        {
        }

        /// <summary>
        /// Clean up selectors and setup version annotation
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="version"></param>
        /// <param name="selectors"></param>
        protected void CleanUpSelectors(string prefix, ApiVersionInfo version, IList<SelectorModel> selectors)
        {
            for (var i = 0; i < selectors.Count; i++)
            {
                var selector = selectors[i];
                if (selector.AttributeRouteModel != null &&
                    !selector.AttributeRouteModel.Template.StartsWith(prefix))
                {
                    selectors.RemoveAt(i);
                    i--;
                }
                else
                {
                    selector.EndpointMetadata.Add(version.Annotation);
                }
            }
        }

        private static void ApplyRoutePrefix(ControllerModel controller, string prefix)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel == null)
                {
                    selector.AttributeRouteModel = new AttributeRouteModel { Template = prefix + "/[controller]" };
                }
                else
                {
                    var routeModel = selector.AttributeRouteModel;
                    if (routeModel.IsAbsoluteTemplate)
                    {
                        continue;
                    }
                    selector.AttributeRouteModel = new AttributeRouteModel(routeModel) { Template = prefix + "/" + routeModel.Template.TrimStart('/') };
                }
            }
        }

        private IReadOnlyList<ApiVersionInfo> GetVersions(IReadOnlyList<object> attributes)
        {
            if (IsApiVersionNeutral(attributes))
            {
                return Versions;
            }

            var result = new List<ApiVersionInfo>(Versions.Count);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (attribute is not IApiVersionProvider versionProvider)
                {
                    continue;
                }

                foreach (var version in versionProvider.Versions)
                {
                    if (result.FindIndex(x => x.Version == version) >= 0)
                    {
                        continue;
                    }
                    result.Add(Versions.First(x => x.Version == version));
                }
            }
            return result;
        }

        private IReadOnlyList<ApiVersionInfo> Versions => _versionInfoProvider.Versions;

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
                if (attribute is IApiVersionProvider)
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
                        if (versionProvider.Versions.Any(v => v == version))
                        {
                            return true;
                        }
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        /// <inheritdoc />
        public virtual void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }
    }
}
