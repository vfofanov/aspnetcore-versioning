﻿namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Defines the behavior of an OData query options convention builder.
    /// </summary>
    public interface IODataQueryOptionsConventionBuilder
    {
        /// <summary>
        ///     Creates and returns an OData query options convention.
        /// </summary>
        /// <param name="settings">The <see cref="ODataQueryOptionSettings">settings</see> used to build the convention.</param>
        /// <returns>A new <see cref="IODataQueryOptionsConvention">OData query options convention</see>.</returns>
        IODataQueryOptionsConvention Build(ODataQueryOptionSettings settings);
    }
}