#nullable enable

using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    /// <summary>
    /// </summary>
    public interface IEdmModelMutator: IEdmModelKeyHolder
    {
        void Run();

        /// <summary>
        ///     Is ignored or not. It can be method for operation, entity clr type, enum, property or enum value
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        bool IsIgnored(MemberInfo? memberInfo);

        /// <summary>
        ///     Is type configuration ignored or not
        /// </summary>
        /// <param name="edmType"></param>
        /// <returns></returns>
        bool IsIgnored(IEdmTypeConfiguration? edmType);

        /// <summary>
        ///     Is property configuration ignored or not
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        bool IsIgnored(PropertyConfiguration? property);
    }
}