using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
    internal delegate bool ODataActionQueryOptionConventionLookup(MethodInfo action, ODataQueryOptionSettings settings,
        out IODataQueryOptionsConvention? convention);
}