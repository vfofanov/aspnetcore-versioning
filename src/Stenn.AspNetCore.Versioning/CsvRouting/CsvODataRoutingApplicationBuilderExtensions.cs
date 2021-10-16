using System;
using Microsoft.AspNetCore.Builder;

namespace Stenn.AspNetCore.Versioning.CsvRouting
{
    /// <summary>
    /// Provides extension methods for <see cref="IApplicationBuilder"/> to add csv routing documentation.
    /// </summary>
    public static class CsvRoutingApplicationBuilderExtensions
    {
        /// <summary>
        /// Use <see cref="CsvRoutingMiddleware"/> route middleware using the given route pattern.
        /// For example, if the given route pattern is "myrouteinfo", then you can send request "~/myrouteinfo" after enabling this middleware.
        /// Please use basic (literal) route pattern with '.csv' at the end. 
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder "/> to use.</param>
        /// <param name="routePattern">The given route pattern.</param>
        /// <param name="csvDelimiter"></param>
        /// <returns>The <see cref="IApplicationBuilder "/>.</returns>
        public static IApplicationBuilder UseCsvDocs(this IApplicationBuilder app, string routePattern = "api-docs.csv", char csvDelimiter = ',')
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (routePattern == null)
            {
                throw new ArgumentNullException(nameof(routePattern));
            }

            return app.UseMiddleware<CsvRoutingMiddleware>(routePattern, csvDelimiter);
        }
    }
}