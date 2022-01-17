using System;
using System.Collections.Generic;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IEdmModelBuilderContext
    {
        IEdmModelMutator Mutator { get; }

        IEnumerable<EntitySetConfiguration> EntitySets { get; }

        IEdmTypeConfiguration? GetTypeConfigurationOrNull(Type type);
    }
}