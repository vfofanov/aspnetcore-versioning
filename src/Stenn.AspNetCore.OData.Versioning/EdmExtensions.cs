using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public static class EdmExtensions
    {
        public static string GetEntitySetName<TController>()
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

        public static ActionConfiguration ReturnsCollectionFromEntitySet<TEntity, TController>(this ActionConfiguration configuration)
            where TEntity : class
            where TController : IODataController<TEntity>
        {
            var entitySetName = GetEntitySetName<TController>();
            return configuration.ReturnsCollectionFromEntitySet<TEntity>(entitySetName);
        }

        public static FunctionConfiguration ReturnsCollectionFromEntitySet<TEntity, TController>(this FunctionConfiguration configuration)
            where TEntity : class
            where TController : IODataController<TEntity>
        {
            var entitySetName = GetEntitySetName<TController>();
            return configuration.ReturnsCollectionFromEntitySet<TEntity>(entitySetName);
        }
    }
}