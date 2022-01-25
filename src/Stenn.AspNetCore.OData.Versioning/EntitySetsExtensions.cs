using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public static class EntitySetsExtensions
    {
        public static IEnumerable<EntitySetConfiguration> GetEntitySetByClrType<T>(this IEdmModelBuilderContext context)
        {
            return context.GetEntitySetByClrType(typeof(T));
        }

        public static IEnumerable<EntitySetConfiguration> GetEntitySetByClrType(this IEdmModelBuilderContext context, Type type)
        {
            return context.EntitySets.GetByClrType(type);
        }

        public static IEnumerable<EntitySetConfiguration> GetByClrType<T>(this IEnumerable<EntitySetConfiguration> sets)
        {
            return sets.GetByClrType(typeof(T));
        }

        public static IEnumerable<EntitySetConfiguration> GetByClrType(this IEnumerable<EntitySetConfiguration> sets, Type type)
        {
            return sets.Where(s => s.ClrType == type).ToList();
        }
        
        public static IEnumerable<EntitySetConfiguration> GetEntitySetByEntityType(this IEdmModelBuilderContext context, EntityTypeConfiguration type)
        {
            return context.EntitySets.GetByEntityType(type);
        }
        public static IEnumerable<EntitySetConfiguration> GetByEntityType(this IEnumerable<EntitySetConfiguration> sets, EntityTypeConfiguration type)
        {
            return sets.Where(s => s.EntityType == type).ToList();
        }

        public static EntitySetConfiguration GetEntitySetByName(this IEdmModelBuilderContext context, string entitySetName)
        {
            return context.EntitySets.GetByName(entitySetName);
        }

        public static EntitySetConfiguration GetByName(this IEnumerable<EntitySetConfiguration> sets, string entitySetName)
        {
            return sets.First(s => s.Name == entitySetName);
        }

        public static EntitySetConfiguration GetEntitySetByName<TController>(this IEdmModelBuilderContext context)
            where TController : IODataController
        {
            var entitySetName = EdmExtensions.GetEntitySetName<TController>();
            return context.GetEntitySetByName(entitySetName);
        }

        public static EntitySetConfiguration GetByName<TController>(this IEnumerable<EntitySetConfiguration> sets)
            where TController : IODataController
        {
            var entitySetName = EdmExtensions.GetEntitySetName<TController>();
            return sets.GetByName(entitySetName);
        }
    }
}