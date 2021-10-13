using System;
using System.Reflection;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Type" /> class.
    /// </summary>
    static partial class TypeExtensions
    {
        private static readonly Type ODataRoutingAttributeType = typeof(ODataAttributeRoutingAttribute);
        private static readonly TypeInfo MetadataController = typeof(MetadataController).GetTypeInfo();
        private static readonly Type Delta = typeof(IDelta);
        private static readonly Type ODataPath = typeof(ODataPath);
        private static readonly Type ODataQueryOptions = typeof(ODataQueryOptions);
        private static readonly Type ODataActionParameters = typeof(ODataActionParameters);

        internal static bool IsODataController(this Type controllerType)
        {
            return Attribute.IsDefined(controllerType, ODataRoutingAttributeType);
        }

        internal static bool IsODataController(this TypeInfo controllerType)
        {
            return Attribute.IsDefined(controllerType, ODataRoutingAttributeType);
        }

        internal static bool IsMetadataController(this TypeInfo controllerType)
        {
            return MetadataController.IsAssignableFrom(controllerType);
        }

        internal static bool IsODataPath(this Type type)
        {
            return ODataPath.IsAssignableFrom(type);
        }

        internal static bool IsODataQueryOptions(this Type type)
        {
            return ODataQueryOptions.IsAssignableFrom(type);
        }

        internal static bool IsODataActionParameters(this Type type)
        {
            return ODataActionParameters.IsAssignableFrom(type);
        }

        internal static bool IsDelta(this Type type)
        {
            return Delta.IsAssignableFrom(type);
        }

        internal static bool IsModelBound(this Type type)
        {
            return ODataPath.IsAssignableFrom(type) ||
                   ODataQueryOptions.IsAssignableFrom(type) ||
                   Delta.IsAssignableFrom(type) ||
                   ODataActionParameters.IsAssignableFrom(type);
        }
    }
}