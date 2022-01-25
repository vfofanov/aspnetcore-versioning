using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.OData.Edm;
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

        /// <inheritdoc />
        public bool CreateOperation<TDeclaringType>(
            IEdmModelOperationHolder holder,
            Expression<Func<TDeclaringType, Task>> operationExpression,
            Action<IEdmModelOperation>? init = null)
        {
            return CreateOperation<TDeclaringType>(holder, operationExpression.Body, init);
        }

        public bool CreateOperation<TDeclaringType>(
            IEdmModelOperationHolder holder,
            Expression<Action<TDeclaringType>> operationExpression,
            Action<IEdmModelOperation>? init = null)
        {
            return CreateOperation<TDeclaringType>(holder, operationExpression.Body, init);
        }

        private bool CreateOperation<TDeclaringType>(IEdmModelOperationHolder holder, Expression operationExpression, Action<IEdmModelOperation>? init)
        {
            var methodCallProvider = operationExpression as MethodCallExpression ??
                                     throw new ArgumentException("Supports only method call expression", nameof(operationExpression));

            var methodInfo = methodCallProvider.Method;

            if (_context.IsIgnored(methodInfo) ||
                _context.IsIgnored(typeof(TDeclaringType)))
            {
                return false;
            }
            var name = GetOperationName(methodInfo);
            var type = GetOperationType(methodInfo);

            IEdmModelOperation op;
            OperationReturnTypeHolder returnTypeHolder;
            switch (type)
            {
                case EdmModelOperationType.Function:
                {
                    var configuration = holder.Function(name);
                    if (!FillParameters(configuration, methodCallProvider, methodInfo))
                    {
                        return false;
                    }
                    op = new EdmModelFunction<TDeclaringType>(methodInfo, configuration);
                    returnTypeHolder = configuration.ToReturnTypeHolder(holder.ClrType);
                }
                    break;
                case EdmModelOperationType.Action:
                {
                    var configuration = holder.Action(name);
                    if (!FillParameters(configuration, methodInfo))
                    {
                        return false;
                    }
                    op = new EdmModelAction<TDeclaringType>(methodInfo, configuration);
                    returnTypeHolder = configuration.ToReturnTypeHolder(holder.ClrType);
                }
                    break;
                default:
                    throw new EdmModelOperationExtractionException(methodInfo, $"Unknown edm operation type {type}");
            }


            var returnTypeExtractResult = FillReturnType(methodInfo, returnTypeHolder);
            init?.Invoke(op);
            if (!returnTypeExtractResult && op.Configuration.ReturnType == null)
            {
                throw new EdmModelOperationExtractionException(methodInfo,
                    "Method has return but it failed to resolve automatically and didn't resolve in init action. " +
                    "Resolve returns in init action");
            }
            return true;
        }

        private bool FillReturnType(MethodInfo methodInfo, OperationReturnTypeHolder holder)
        {
            var type = methodInfo.ReturnType;
            if (OperationReturnTypeExtensions.ReturnsVoid(type))
            {
                holder.ReturnsVoid();
                return true;
            }

            type = OperationReturnTypeExtensions.UnwrapTask(type);
            bool isCollection;
            (type, isCollection) = OperationReturnTypeExtensions.UnwrapCollection(type);

            if (_context.IsIgnored(type))
            {
                return false;
            }

            var edmType = _context.GetTypeConfigurationOrNull(type);
            if (edmType is null)
            {
                //NOTE: It will implicitly add new type
                holder.Returns(type, isCollection);
                return true;
            }

            switch (edmType.Kind)
            {
                case EdmTypeKind.Enum:
                case EdmTypeKind.Primitive:
                case EdmTypeKind.Complex:
                    holder.Returns(type, isCollection);
                    break;
                case EdmTypeKind.Entity:
                    var sets = _context.EntitySets.Where(s => s.ClrType == type).ToList();
                    if (sets.Count == 1)
                    {
                        var set = sets[0];
                        holder.ReturnsFromEntitySet(type, set.Name, isCollection);
                        break;
                    }
                    if (holder.BindedClrType != type)
                    {
                        return false;
                    }
                    //NOTE: Special case when we have multiple entity sets for one clr type
                    //and we binded operation to one of this types
                    holder.ReturnsViaEntitySetPath(type, holder.Configuration.BindingParameter.Name, isCollection);
                    break;
            }
            return true;
        }

        private bool FillParameters(FunctionConfiguration configuration,
            MethodCallExpression methodCallProvider, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            for (var i = 0; i < methodCallProvider.Arguments.Count; i++)
            {
                var paramInfo = parameters[i];
                if (_context.IsIgnored(paramInfo.ParameterType))
                {
                    return false;
                }
                var paramType = paramInfo.ParameterType;
                var parameterName = GetFunctionParameterName(i, paramInfo);
                var paramConfiguration = CreateParameter(configuration, paramType, parameterName);
                var argExpression = methodCallProvider.Arguments[i];
                InitParameterByAttribute(paramInfo, paramConfiguration);
                InitFunctionParameter(methodInfo, argExpression, paramConfiguration);
            }
            return true;
        }

        private static void InitFunctionParameter(MethodInfo methodInfo, Expression argExpression, ParameterConfiguration paramConfiguration)
        {
            switch (argExpression)
            {
                case MethodCallExpression argMethodCallExpression:
                    if (argMethodCallExpression.Method.DeclaringType != typeof(EdmOp))
                    {
                        goto default;
                    }
                    if (argMethodCallExpression.Arguments.Count == 1)
                    {
                        var edmOpParamExpression = argMethodCallExpression.Arguments[0];
                        switch (edmOpParamExpression)
                        {
                            case Expression<Action<ParameterConfiguration>> initExpression:
                                var initParam = initExpression.Compile();
                                initParam.Invoke(paramConfiguration);
                                break;
                            default:
                                throw new EdmModelOperationExtractionException(methodInfo,
                                    "Function's registaration failed." +
                                    "OData function's parameter initialized by EdmOp.Param<T>(init) must be initialized with not null init of type 'Action<ParameterConfiguration>'");
                        }
                    }
                    break;
                case ConstantExpression:
                    //NOTE: Skip 'default' keyword initialization
                    break;
                default:
                    throw new EdmModelOperationExtractionException(methodInfo,
                        "Function's registaration failed." +
                        "OData function's parameter can be initialized by 'default' keyword or 'Stenn.AspNetCore.OData.Versioning.EdmOp' members only");
            }
        }

        private bool FillParameters(ActionConfiguration configuration, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 1)
            {
                throw new EdmModelOperationExtractionException(methodInfo,
                    "Has more than one parameter. " +
                    "OData action method can to have one parameter of 'ODataActionParameters' type or 'ODataUntypedActionParameters' type");
            }

            foreach (var parameterInfo in parameters)
            {
                if (parameterInfo.ParameterType == typeof(ODataActionParameters))
                {
                    var actionParams = GetActionParamsType(parameterInfo) ??
                                       throw new EdmModelOperationExtractionException(methodInfo,
                                           "Has parameter of 'ODataActionParameters' type with undefined 'ODataActionParams' parameter's attribute. " +
                                           "OData action method can to have one parameter of 'ODataActionParameters' type with one 'ODataActionParams' parameter's attribute");

                    var actionParamsType = actionParams.GetType();
                    foreach (var paramInfo in actionParamsType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
                    {
                        var paramType = paramInfo.PropertyType;
                        if (_context.IsIgnored(paramType))
                        {
                            return false;
                        }

                        var parameterName = GetActionParameterName(paramInfo);
                        var paramConfiguration = CreateParameter(configuration, paramType, parameterName);

                        InitParameterByAttribute(paramInfo, paramConfiguration);
                        actionParams.InitParameter(paramInfo, paramConfiguration);
                    }
                }
                else if (parameterInfo.ParameterType == typeof(ODataUntypedActionParameters))
                {
                    //NOTE: Just skip ODataUntypedActionParameters
                }
                else
                {
                    throw new EdmModelOperationExtractionException(methodInfo,
                        $"Has parameter with '{parameterInfo.ParameterType.FullName}'. " +
                        "Odata action method can to have one parameter of 'ODataActionParameters' type or 'ODataUntypedActionParameters' type");
                }
            }
            return true;
        }

        protected virtual ParameterConfiguration CreateParameter(OperationConfiguration configuration, Type paramType, string parameterName)
        {
            return paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                ? configuration.CollectionParameter(paramType, parameterName)
                : configuration.Parameter(paramType, parameterName);
        }

        private static ODataActionParams? GetActionParamsType(ParameterInfo paramInfo)
        {
            var oDataParamsList = paramInfo.GetCustomAttributes().OfType<ODataActionParams>().ToList();
            return oDataParamsList.Count != 1 ? null : oDataParamsList[0];
        }

        protected virtual void InitParameterByAttribute(ICustomAttributeProvider paramInfo, ParameterConfiguration paramConfiguration)
        {
            foreach (var initializer in paramInfo.GetCustomAttributes(true).OfType<IParameterConfigurationInitializer>())
            {
                initializer.Initialize(paramConfiguration);
            }
        }

        protected virtual string GetOperationName(MethodInfo method)
        {
            return method.Name;
        }

        /// <summary>
        ///     Gets parameter's name for <see cref="FunctionConfiguration" />
        /// </summary>
        /// <param name="index">Parameter's index</param>
        /// <param name="info">Parameter's metadata</param>
        /// <returns></returns>
        protected virtual string GetFunctionParameterName(int index, ParameterInfo info)
        {
            return info.Name ?? $"p{index}";
        }

        /// <summary>
        ///     Gets parameter's name for <see cref="ActionConfiguration" />
        /// </summary>
        /// <param name="info">Parameter's metadata</param>
        /// <returns></returns>
        protected virtual string GetActionParameterName(PropertyInfo info)
        {
            return info.Name;
        }

        protected virtual EdmModelOperationType GetOperationType(MethodInfo methodInfo)
        {
            var httpMethods = methodInfo.GetCustomAttributes<HttpMethodAttribute>().ToList();
            if (httpMethods.Count == 0)
            {
                throw new EdmModelOperationExtractionException(methodInfo,
                    "Method doesn't have any HttpMethodAttribute. " +
                    "Unable to get type of odata operation. Mark method with one of attribute: [HttpGet] for function, [HttpPost] - for Operation");
            }
            if (httpMethods.Count > 1)
            {
                throw new EdmModelOperationExtractionException(methodInfo,
                    "Method has multiple HttpMethodAttribute. " +
                    "Unable to get type of odata operation. Mark method with one of attribute: [HttpGet] for function, [HttpPost] - for Operation");
            }

            switch (httpMethods[0])
            {
                case HttpGetAttribute:
                    return EdmModelOperationType.Function;
                case HttpPostAttribute:
                    return EdmModelOperationType.Action;
                default:
                    throw new EdmModelOperationExtractionException(methodInfo,
                        $"Method has unmatched attribute {httpMethods[0].GetType().Name}. " +
                        "Unable to get type of odata operation. Mark method with one of attribute: [HttpGet] for function, [HttpPost] - for Operation");
            }
        }
    }


    public abstract class EdmModelOperation<TConfiguration, TDeclaringType> : IEdmModelOperation
        where TConfiguration : OperationConfiguration
    {
        public EdmModelOperation(TConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TConfiguration Configuration { get; }

        /// <inheritdoc />
        public abstract EdmModelOperationType Type { get; }

        /// <inheritdoc />
        OperationConfiguration IEdmModelOperation.Configuration => Configuration;
    }
}