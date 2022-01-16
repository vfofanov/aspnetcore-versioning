using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IEdmModelBuilderContext
    {
        IEdmModelMutator Mutator { get; }
    }
}