#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.Versioning;

namespace Stenn.AspNetCore.OData.Versioning
{
    public static class ODataEndpointExtensions
    {
        public static ApiVersion? GetODataApiVersion(this ODataControllerActionContext context)
        {
            
            return GetODataApiVersion(context.Model);
        }

        public static ApiVersion? GetODataApiVersion(this Endpoint endpoint)
        {
            var metadata = GetODataRoutingMetadata(endpoint);
            return GetODataApiVersion(metadata);
        }

        public static ApiVersion? GetODataApiVersion(this ControllerModel controller)
        {
            var metadata = GetODataRoutingMetadata(controller);
            return GetODataApiVersion(metadata);
        }
        
        public static IODataRoutingMetadata? GetODataRoutingMetadata(this ControllerModel controller)
        {
            return GetODataRoutingMetadata(controller.Attributes);
        }
        
        public static IODataRoutingMetadata? GetODataRoutingMetadata(this Endpoint endpoint)
        {
            return GetODataRoutingMetadata(endpoint.Metadata);
        }

        public static IODataRoutingMetadata? GetODataRoutingMetadata(IEnumerable<object> attributes)
        {
            return attributes.OfType<IODataRoutingMetadata>().FirstOrDefault();
        }
        
        public static ApiVersion? GetODataApiVersion(this IODataRoutingMetadata? metadata)
        {
            return metadata?.Model.GetODataApiVersion();
        }

        public static ApiVersion? GetODataApiVersion(this IEdmModel? model)
        {
            return model?.GetAnnotationValue<ApiVersionAnnotation>(model)?.ApiVersion;
        }

        public static void SetApiVersion(this IEdmModel model, ApiVersion version)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }
            model.SetAnnotationValue(model, new ApiVersionAnnotation(version));
        }

        public static bool IsODataApiVersionMatch(this ICommonModel model, ApiVersion version)
        {
            return IsODataApiVersionMatch(model.Attributes, version);
        }

        public static bool IsODataApiVersionMatch(this Endpoint endpoint, ApiVersion apiVersion)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            return IsODataApiVersionMatch(endpoint.Metadata, apiVersion);
        }
        
        public static bool IsODataApiVersionMatch(this EndpointMetadataCollection metadata, ApiVersion apiVersion)
        {
            return IsODataApiVersionMatch((IReadOnlyList<object>)metadata, apiVersion);
        }
        
        private static bool IsODataApiVersionMatch(IReadOnlyCollection<object> metadata, ApiVersion apiVersion)
        {
            var apiVersionsNeutral = metadata.OfType<IApiVersionNeutral>().ToList();
            if (apiVersionsNeutral.Count != 0)
            {
                return true;
            }

            var apiVersions = metadata.OfType<IApiVersionProvider>().ToList();
            if (apiVersions.Count == 0)
            {
                // If no [ApiVersion] on the controller,
                // Let's simply return true, it means it can work the input version or any version.
                return true;
            }

            for (var i = 0; i < apiVersions.Count; i++)
            {
                var item = apiVersions[i];
                if (item.Versions.Contains(apiVersion))
                {
                    return true;
                }
            }

            return false;
        }
    }
}