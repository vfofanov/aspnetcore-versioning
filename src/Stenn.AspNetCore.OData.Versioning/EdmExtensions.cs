using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public static class EdmExtensions
    {
        public static string GetEntitySetName<TController>()
            where TController : IODataController
        {
            return GetEntitySetName(typeof(TController));
        }

        public static string GetEntitySetName(Type controllerType)
        {
            //Use controller name convention
            var typeName = controllerType.Name;
            if (typeName.Length > 10 && typeName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
            {
                return typeName[..^10];
            }
            return typeName;
        }
    }
}