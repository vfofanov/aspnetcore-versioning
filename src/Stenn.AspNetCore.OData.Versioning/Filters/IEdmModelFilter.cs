#nullable enable

using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IEdmModelFilter : IEdmModelKeyHolder
    {
        /// <summary>
        /// Is filter enabled for declaration model or not.
        /// </summary>
        bool ForRequestModelOnly { get; }

        /// <summary>
        /// Is ignored or not. It can be method for operation, entity clr type, enum, property or enum value
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        bool IsIgnored(MemberInfo memberInfo);

        /// <summary>
        /// Is edm type ignored or not. Used as additional to <see cref="IsIgnored(System.Reflection.MemberInfo)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsIgnored(IEdmTypeConfiguration type)
        {
            return IsIgnored(type.ClrType);
        }

        /// <summary>
        /// Is edm type ignored or not. Used as additional to <see cref="IsIgnored(System.Reflection.MemberInfo)"/>
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool IsIgnored(PropertyConfiguration property)
        {
            return IsIgnored(property.RelatedClrType) || IsIgnored(property.PropertyInfo);
        }
    }
}