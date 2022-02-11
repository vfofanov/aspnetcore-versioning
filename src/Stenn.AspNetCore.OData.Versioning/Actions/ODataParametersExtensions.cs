#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public static class ODataParametersExtensions
    {
        private static readonly ConcurrentDictionary<Type, IODataActionParametersMapper> ParamsMappers = new();

        public static T Get<T>(this Dictionary<string, object>? parameters)
            where T : ODataActionParams, new()
        {
            var mapper = ParamsMappers.GetOrAdd(typeof(T), ODataActionParametersMapper<T>.Create);
            return (T)mapper.Map(parameters ?? new Dictionary<string, object>());
        }
    }
}