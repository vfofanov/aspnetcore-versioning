#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public static class ODataParametersExtensions
    {
        private static readonly ConcurrentDictionary<Type, IODataActionParametersMapper> ParamsMappers = new();
        public static T Get<T>(this Dictionary<string, object> parameters)
            where T : ODataActionParams, new()
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters are null. Check action's parameters names and missed parameters");
            }

            var mapper = ParamsMappers.GetOrAdd(typeof(T), ODataActionParametersMapper<T>.Create);
            return (T)mapper.Map(parameters);
        }
    }
}