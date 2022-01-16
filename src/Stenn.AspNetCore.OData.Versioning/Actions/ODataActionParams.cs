using System;
using System.Reflection;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ODataActionParams : Attribute
    {
        public virtual void InitParameter(PropertyInfo propertyInfo, ParameterConfiguration configuration)
        {
        }
    }
}