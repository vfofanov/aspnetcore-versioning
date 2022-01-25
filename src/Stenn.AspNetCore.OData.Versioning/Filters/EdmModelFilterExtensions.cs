using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public static class EdmModelFilterExtensions
    {
        /// <summary>
        /// Is edm type ignored or not. Used as additional to <see cref="IModelFilter.IsIgnored(System.Reflection.MemberInfo)"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIgnored(this IModelFilter filter, IEdmTypeConfiguration type)
        {
            return filter.IsIgnored(type.ClrType);
        }

        /// <summary>
        /// Is edm type ignored or not. Used as additional to <see cref="IModelFilter.IsIgnored(System.Reflection.MemberInfo)"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsIgnored(this IModelFilter filter, PropertyConfiguration property)
        {
            return filter.IsIgnored(property.RelatedClrType) || filter.IsIgnored(property.PropertyInfo);
        }
    }
}