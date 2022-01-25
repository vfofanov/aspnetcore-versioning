using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public interface IEdmModelOperationExtractor
    {
        bool CreateOperation<TDeclaringType>(
            IEdmModelOperationHolder holder,
            Expression<Func<TDeclaringType, Task>> operationExpression,
            Action<IEdmModelOperation>? init = null);
        
        bool CreateOperation<TDeclaringType>(
            IEdmModelOperationHolder holder,
            Expression<Action<TDeclaringType>> operationExpression,
            Action<IEdmModelOperation>? init = null);
    }
}