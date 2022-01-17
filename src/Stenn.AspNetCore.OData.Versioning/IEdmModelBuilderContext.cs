using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IEdmModelBuilderContext
    {
        IEnumerable<EntitySetConfiguration> EntitySets { get; }

        /// <summary>
        ///     Is ignored or not. It can be method for operation, entity clr type, enum, property or enum value
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        bool IsIgnored(MemberInfo? memberInfo);
        
        IEdmTypeConfiguration? GetTypeConfigurationOrNull(Type type);
    }
}