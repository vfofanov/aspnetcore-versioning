﻿using System;

namespace Microsoft.AspNet.OData
{
    /// <summary>
    ///     Provides metadata about the original type when a type is substituted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class OriginalTypeAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OriginalTypeAttribute" /> class.
        /// </summary>
        /// <param name="type">The original type.</param>
        public OriginalTypeAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        ///     Gets the original type.
        /// </summary>
        /// <value>The original <see cref="Type">type</see>.</value>
        public Type Type { get; }
    }
}