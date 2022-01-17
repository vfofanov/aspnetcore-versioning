using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public static class OperationReturnTypeExtensions
    {
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
            bool isCollection = false;
            if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                if (typeDefinition == typeof(IEnumerable<>) || typeDefinition == typeof(IQueryable<>))
                {
                    isCollection = true;
                    type = type.GetGenericArguments()[0];
                }
            }
            return (type, isCollection);
        }
    }
}