using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public interface IEdmModelOperationHolder
    {
        ActionConfiguration Action(string name);
        
        FunctionConfiguration Function(string name);
    }
}