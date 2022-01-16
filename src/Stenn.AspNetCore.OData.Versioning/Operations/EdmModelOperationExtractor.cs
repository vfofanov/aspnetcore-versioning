using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning.Actions;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public class EdmModelOperationExtractor : IEdmModelOperationExtractor
    {
        private readonly IEdmModelBuilderContext _context;

        public EdmModelOperationExtractor(IEdmModelBuilderContext context)
        {
            _context = context;
        }

        public bool CreateOperation<TDeclaringType>(
            IEdmModelOperationHolder holder,
            Expression<Action<TDeclaringType>> operationExpression, 
            Action<EdmModelOperation<TDeclaringType>>? init=null)
        {
            var methodCallProvider = operationExpression.Body as MethodCallExpression ??
                                     throw new ArgumentException("Supports only method call expression", nameof(operationExpression));
            
            var methodInfo = methodCallProvider.Method;

            if (_context.Mutator.IsIgnored(methodInfo) ||
                _context.Mutator.IsIgnored(typeof(TDeclaringType)))
            {
                return false;
            }
            var name = GetOperationName(methodInfo);
            var type = GetOperationType(methodInfo);

            EdmModelOperation<TDeclaringType> op;
            switch (type)
            {
                case EdmModelOperationType.Function:
                {
                    var configuration = holder.Function(name);
                    // TODO: Handle return type
                    if (FillParameters(configuration, methodCallProvider, methodInfo))
                    {
                        return false;
                    }
                    op = new EdmModelFunction<TDeclaringType>(configuration);
                }
                    break;
                case EdmModelOperationType.Action:
                {
                    var configuration = holder.Action(name);
                    //TODO: Handle return type
                    if (!FillParameters(configuration, methodInfo))
                    {
                        return false;
                    }
                    op = new EdmModelAction<TDeclaringType>(configuration);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown edm operation type {type}");

            }
            
            init?.Invoke(op);
            return true;
        }

        private bool FillParameters(FunctionConfiguration configuration,
            MethodCallExpression methodCallProvider, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            for (var i = 0; i < methodCallProvider.Arguments.Count; i++)
            {
                var paramInfo = parameters[i];
                if (_context.Mutator.IsIgnored(paramInfo.ParameterType))
                {
                    return false;
                }
                var argExpression = methodCallProvider.Arguments[i];
                        
            }
            return true;
        }

        private bool FillParameters(ActionConfiguration configuration, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 1)
            {
                throw new ApplicationException($"Method {methodInfo.DeclaringType?.FullName}{methodInfo.Name} have more than one parameter. " +
                                               "Odata action method can to have one parameter of 'ODataActionParameters' type or 'ODataUntypedActionParameters' type");
            }

            foreach (var parameterInfo in parameters)
            {
                if (parameterInfo.ParameterType == typeof(ODataActionParameters))
                {
                    var actionParams = GetActionParamsType(parameterInfo) ??
                                       throw new ApplicationException(
                                           $"Method {methodInfo.DeclaringType?.FullName}{methodInfo.Name} have parameter of 'ODataActionParameters' type with undefined 'ODataActionParams' parameter's attribute. " +
                                           "Odata action method can to have one parameter of 'ODataActionParameters' type with one 'ODataActionParams' parameter's attribute");

                    var actionParamsType = actionParams.GetType();
                    foreach (var paramInfo in actionParamsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var paramType = paramInfo.PropertyType;
                        if (_context.Mutator.IsIgnored(paramType))
                        {
                            return false;
                        }
                        
                        var parameterName = GetParameterName(paramInfo);

                        var paramConfiguration = paramType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            ? configuration.CollectionParameter(paramType, parameterName)
                            : configuration.Parameter(paramType, parameterName);

                        actionParams.InitParameter(paramInfo, paramConfiguration);
                    }
                }
                else if (parameterInfo.ParameterType == typeof(ODataUntypedActionParameters))
                {
                    //NOTE: Just skip ODataUntypedActionParameters
                }
                else
                {
                    throw new ApplicationException(
                        $"Method {methodInfo.DeclaringType?.FullName}{methodInfo.Name} have parameter with '{parameterInfo.ParameterType.FullName}'. " +
                        "Odata action method can to have one parameter of 'ODataActionParameters' type or 'ODataUntypedActionParameters' type");
                }
            }
            return true;
        }

        private static ODataActionParams? GetActionParamsType(ParameterInfo paramInfo)
        {
            var oDataParamsList = paramInfo.GetCustomAttributes().OfType<ODataActionParams>().ToList();
            return oDataParamsList.Count != 1 ? null : oDataParamsList[0];
        }

        protected virtual string GetOperationName(MethodInfo method)
        {
            return method.Name;
        }
        protected virtual string GetParameterName(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }
        
        protected virtual EdmModelOperationType GetOperationType(MethodInfo method)
        {
            var httpMethods = method.GetCustomAttributes<HttpMethodAttribute>().ToList();
            if (httpMethods.Count == 0)
            {
                throw new ArgumentException($"Method {method.DeclaringType?.FullName}{method.Name} doesn't have any HttpMethodAttribute. " +
                                            "Unable to get type of odata operation. Mark method with one of attribute: [HttpGet] for function, [HttpPost] - for Operation");
            }
            if (httpMethods.Count > 1)
            {
                throw new ArgumentException($"Method {method.DeclaringType?.FullName}{method.Name} have multiple HttpMethodAttribute. " +
                                            "Unable to get type of odata operation. Mark method with one of attribute: [HttpGet] for function, [HttpPost] - for Operation");
            }

            switch (httpMethods[0])
            {
                case HttpGetAttribute:
                    return EdmModelOperationType.Function;
                case HttpPostAttribute:
                    return EdmModelOperationType.Action;
                default:
                    throw new ArgumentException(
                        $"Method {method.DeclaringType?.FullName}{method.Name} have unmatched attribute {httpMethods[0].GetType().Name}. " +
                        "Unable to get type of odata operation. Mark method with one of attribute: [HttpGet] for function, [HttpPost] - for Operation");
            }
        }
    }


    public abstract  class EdmModelOperation<TConfiguration, TDeclaringType>:EdmModelOperation<TDeclaringType>
    {
        public EdmModelOperation(TConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TConfiguration Configuration { get; }
        
    }
    public abstract class EdmModelOperation<TDeclaringType>
    {
        public abstract EdmModelOperationType Type { get; }
    }

    public class EdmModelFunction<TDeclaringType> : EdmModelOperation<FunctionConfiguration, TDeclaringType>
    {
        /// <inheritdoc />
        public EdmModelFunction(FunctionConfiguration configuration) 
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override EdmModelOperationType Type => EdmModelOperationType.Function;
    }

    public class EdmModelAction<TDeclaringType> : EdmModelOperation<ActionConfiguration, TDeclaringType>
    {
        /// <inheritdoc />
        public EdmModelAction(ActionConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override EdmModelOperationType Type => EdmModelOperationType.Action;
    }
}