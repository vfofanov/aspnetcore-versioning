#nullable enable

using System.Reflection;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IModelFilter : IEdmModelKeyHolder
    {
        /// <summary>
        /// Is ignored or not. It can be method for operation, entity clr type, enum, property or enum value
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        bool IsIgnored(MemberInfo memberInfo);
    }
}