using System;
using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public static class ODataParametersExtensions
    {
        private static readonly MethodInfo CollectionParameterMethod =
            typeof(FunctionConfiguration).GetMethod(nameof(OperationConfiguration.CollectionParameter)) ??
            throw new ApplicationException("Can't find method 'OperationConfiguration.CollectionParameter'");
        public static ParameterConfiguration CollectionParameter(this OperationConfiguration action,
            Type parameterType,
            string parameterName)
        {
            var itemType = parameterType.GetGenericArguments()[0];
            var generic = CollectionParameterMethod.MakeGenericMethod(itemType);
            var configuration = (ParameterConfiguration?)generic.Invoke(action, new object?[] { parameterName });
            return configuration!;
        }
    }
}