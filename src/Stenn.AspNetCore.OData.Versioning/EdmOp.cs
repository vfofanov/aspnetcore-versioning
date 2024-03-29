﻿#nullable enable

using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public class EdmOp
    {
        /// <summary>
        ///     Mark for edm function parameter
        /// </summary>
        /// <param name="init">Parameter initialization action</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Param<T>(Action<ParameterConfiguration> init)
        {
            throw new InvalidOperationException("Invalid method call. Use it only in expression");
        }
    }
}