using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public static class OperationExtractorExtensions
    {
        private static readonly MethodInfo CollectionParameterMethod =
            typeof(FunctionConfiguration).GetMethod(nameof(OperationConfiguration.CollectionParameter)) ??
            throw new ApplicationException("Can't find method 'OperationConfiguration.CollectionParameter'");
        public static ParameterConfiguration CollectionParameter(this OperationConfiguration action, Type itemType, string parameterName)
        {
            var generic = CollectionParameterMethod.MakeGenericMethod(itemType);
            var configuration = (ParameterConfiguration?)generic.Invoke(action, new object?[] { parameterName });
            return configuration!;
        }
        
        public static OperationReturnTypeHolder ToReturnTypeHolder(this ActionConfiguration operation, Type? holderClrType)
        {
            return new OperationReturnTypeHolder.Action(operation, holderClrType);
        }

        public static OperationReturnTypeHolder ToReturnTypeHolder(this FunctionConfiguration operation, Type? holderClrType)
        {
            return new OperationReturnTypeHolder.Function(operation,holderClrType);
        }

        public static bool ReturnsVoid(Type type)
        {
            return type == typeof(void) || 
                   type == typeof(Task) || 
                   type == typeof(ValueTask);
        }
        
        public static Type UnwrapTask(Type type)
        {
            if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                if (typeDefinition == typeof(Task<>) || typeDefinition == typeof(ValueTask<>))
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            return type;
        }

        public static (Type type, bool isCollection) UnwrapCollection(Type type)
        {
            var isCollection = false;
            if (!type.IsGenericType)
            {
                return (type, isCollection);
            }

            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length > 1)
            {
                return (type, isCollection);
            }

            var itemType = type.GetGenericArguments()[0];
            var ienumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
            if (ienumerableType.IsAssignableFrom(type))
            {
                isCollection = true;
                type = itemType;
            }
            return (type, isCollection);
        }
    }
}