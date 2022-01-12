using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public static class ODataParametersExtensions
    {
        public static FunctionConfiguration AddParameter<T>(this FunctionConfiguration function, string name,
            Action<ParameterConfiguration>? init = null)
        {
            var parameterConfiguration = function.Parameter<T>(name);
            init?.Invoke(parameterConfiguration);
            return function;
        }
        
        public static FunctionConfiguration AddEntityParameter<T>(this FunctionConfiguration function, string name,
            Action<ParameterConfiguration>? init = null) where T : class
        {
            var parameterConfiguration = function.EntityParameter<T>(name);
            init?.Invoke(parameterConfiguration);
            return function;
        }
        
        public static FunctionConfiguration AddCollectionParameter<T>(this FunctionConfiguration function, string name,
            Action<ParameterConfiguration>? init = null)
        {
            var parameterConfiguration = function.CollectionParameter<T>(name);
            init?.Invoke(parameterConfiguration);
            return function;
        }
        
        public static FunctionConfiguration AddCollectionEntityParameter<T>(this FunctionConfiguration function, string name,
            Action<ParameterConfiguration>? init = null) where T : class
        {
            var parameterConfiguration = function.CollectionEntityParameter<T>(name);
            init?.Invoke(parameterConfiguration);
            return function;
        }
        
        public static ActionConfiguration AddParameter<T>(this ActionConfiguration action, ODataActionParameter<T> parameter,
            Action<ParameterConfiguration>? init = null)
        {
            var parameterConfiguration = action.Parameter<T>(parameter.Name);
            init?.Invoke(parameterConfiguration);
            return action;
        }

        public static ActionConfiguration AddParameter<T>(this ActionConfiguration action, ODataActionCollectionParameter<T> parameter,
            Action<ParameterConfiguration>? init = null)
        {
            var parameterConfiguration = action.CollectionParameter<T>(parameter.Name);
            init?.Invoke(parameterConfiguration);
            return action;
        }
        
        public static ActionConfiguration AddParameter<T>(this ActionConfiguration action, ODataActionEntityParameter<T> parameter,
            Action<ParameterConfiguration>? init = null) 
            where T : class
        {
            var parameterConfiguration = action.EntityParameter<T>(parameter.Name);
            init?.Invoke(parameterConfiguration);
            return action;
        }

        public static ActionConfiguration AddParameter<T>(this ActionConfiguration action, ODataActionCollectionEntityParameter<T> parameter,
            Action<ParameterConfiguration>? init = null) 
            where T : class
        {
            var parameterConfiguration = action.CollectionEntityParameter<T>(parameter.Name);
            init?.Invoke(parameterConfiguration);
            return action;
        }

        public static T? Get<T>(this ODataActionParameters parameters, IODataActionParameter<T> parameter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters are null. Check action's parameters names and missed parameters");
            }
            return (T?)parameters[parameter.Name];
        }

        public static IEnumerable<T>? Get<T>(this ODataActionParameters parameters, IODataActionCollectionParameter<T> parameter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters are null. Check action's parameters names and missed parameters");
            }
            return (IEnumerable<T>?)parameters[parameter.Name];
        }
    }
}