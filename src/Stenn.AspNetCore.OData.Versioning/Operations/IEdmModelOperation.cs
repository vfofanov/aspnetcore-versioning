using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public interface IEdmModelOperation
    {
        EdmModelOperationType Type { get; }
        OperationConfiguration Configuration { get; }
    }
}