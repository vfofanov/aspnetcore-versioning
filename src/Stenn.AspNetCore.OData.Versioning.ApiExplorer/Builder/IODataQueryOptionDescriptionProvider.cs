using Microsoft.AspNetCore.OData.Query;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Defines the behavior of an object that provides descriptions for OData query options.
    /// </summary>
    public interface IODataQueryOptionDescriptionProvider
    {
        /// <summary>
        ///     Creates and returns a description for the specified OData query option using the provided context.
        /// </summary>
        /// <param name="queryOption">The <see cref="AllowedQueryOptions">query option</see> to provide a description for.</param>
        /// <param name="context">The <see cref="ODataQueryOptionDescriptionContext">context</see> used to create the description.</param>
        /// <returns>The description for the specified <paramref name="queryOption">query option</paramref>.</returns>
        string Describe(AllowedQueryOptions queryOption, ODataQueryOptionDescriptionContext context);
    }
}