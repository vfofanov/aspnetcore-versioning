using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public interface IEdmModelOperationHolder
    {
        Type? ClrType { get; }
        ActionConfiguration Action(string name);
        
        FunctionConfiguration Function(string name);
    }
}