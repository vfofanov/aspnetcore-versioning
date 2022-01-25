using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public class EdmModelAction<TDeclaringType> : EdmModelOperation<ActionConfiguration, TDeclaringType>
    {
        /// <inheritdoc />
        public EdmModelAction(MethodInfo methodInfo, ActionConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override EdmModelOperationType Type => EdmModelOperationType.Action;
    }
}