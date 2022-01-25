using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    internal sealed class ODataActionParametersMapper<T> : IODataActionParametersMapper
        where T : class, new()
    {
        private readonly Action<T, Dictionary<string, object>>[] _actions;

        private ODataActionParametersMapper(Action<T, Dictionary<string, object>>[] actions)
        {
            _actions = actions;
        }

        /// <inheritdoc />
        object IODataActionParametersMapper.Map(Dictionary<string, object> parameters)
        {
            return Map(parameters);
        }

        private static Action<T, Dictionary<string, object>> Map(PropertyInfo property)
        {
            var name = property.Name;
            var camelCasePropertyName = char.ToLowerInvariant(name[0]) + name[1..];

            if (name == camelCasePropertyName)
            {
                return (result, parameters) =>
                {
                    if (parameters.TryGetValue(name, out var val))
                    {
                        property.SetMethod?.Invoke(result, new[] { val });
                    }
                };
            }
            return (result, parameters) =>
            {
                if (parameters.TryGetValue(name, out var val) ||
                    parameters.TryGetValue(camelCasePropertyName, out val))
                {
                    property.SetMethod?.Invoke(result, new[] { val });
                }
            };
        }

        public static IODataActionParametersMapper Create(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            return new ODataActionParametersMapper<T>(properties.Select(Map).ToArray());
        }

        public T Map(Dictionary<string, object> parameters)
        {
            var result = new T();
            for (var i = 0; i < _actions.Length; i++)
            {
                _actions[i].Invoke(result, parameters);
            }
            return result;
        }
    }
}