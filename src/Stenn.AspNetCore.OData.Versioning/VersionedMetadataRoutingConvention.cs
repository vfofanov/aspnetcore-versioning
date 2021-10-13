using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.OData.Routing.Template;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///     The convention for $metadata.
    /// </summary>
    public sealed class VersionedMetadataRoutingConvention<TMetadataController> : IODataControllerActionConvention
        where TMetadataController : MetadataControllerBase
    {
        private static readonly TypeInfo MetadataTypeInfo = typeof(TMetadataController).GetTypeInfo();

        /// <summary>
        ///     Gets the order value for determining the order of execution of conventions.
        ///     Metadata routing convention has 0 order.
        /// </summary>
        public int Order => 0;

        /// <inheritdoc />
        public bool AppliesToController(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            // This convention only applies to "MetadataController".
            return context.Controller.ControllerType == MetadataTypeInfo;
        }

        /// <inheritdoc />
        public bool AppliesToAction(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            Debug.Assert(context.Controller != null);
            Debug.Assert(context.Action != null);

            var action = context.Action;
            var actionName = action.ActionName;

            switch (actionName)
            {
                case nameof(MetadataControllerBase.GetMetadata):
                {
                    // for ~$metadata
                    var template = new ODataPathTemplate(MetadataSegmentTemplate.Instance);
                    action.AddSelector(HttpMethods.Get, context.Prefix, context.Model, template, context.Options?.RouteOptions);
                    return true;
                }
                case nameof(MetadataControllerBase.GetServiceDocument):
                {
                    // GET for ~/
                    var template = new ODataPathTemplate();
                    action.AddSelector(HttpMethods.Get, context.Prefix, context.Model, template, context.Options?.RouteOptions);
                    return true;
                }
                case nameof(MetadataControllerBase.GetOptions):
                {
                    //OPTIONS for ~/
                    var template = new ODataPathTemplate();
                    action.AddSelector(HttpMethods.Options, context.Prefix, context.Model, template, context.Options?.RouteOptions);
                    return true;
                }
                default:
                    return false;
            }
        }
    }
}