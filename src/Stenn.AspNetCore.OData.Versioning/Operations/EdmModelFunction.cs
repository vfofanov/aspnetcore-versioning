using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public class EdmModelFunction<TDeclaringType> : EdmModelOperation<FunctionConfiguration, TDeclaringType>
    {
        /// <inheritdoc />
        public EdmModelFunction(MethodInfo methodInfo, FunctionConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override EdmModelOperationType Type => EdmModelOperationType.Function;
    }
}