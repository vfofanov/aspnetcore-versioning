using System;
using System.Linq.Expressions;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public interface IEdmModelOperationExtractor
    {
        bool CreateOperation<TDeclaringType>(
            IEdmModelOperationHolder holder,
            Expression<Action<TDeclaringType>> operationExpression, 
            Action<EdmModelOperation<TDeclaringType>>? init=null);
    }
}