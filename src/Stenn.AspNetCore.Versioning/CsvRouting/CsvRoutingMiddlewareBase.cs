#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Stenn.AspNetCore.Versioning.CsvRouting
{
    public abstract class CsvRoutingMiddlewareBase
    {
        protected char Delimiter { get; }
        private readonly char[] _escapingChars;
        private readonly RequestDelegate _next;
        private readonly string _routePattern;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvRoutingMiddleware" /> class.
        /// </summary>
        /// <param name="routePattern"></param>
        /// <param name="next">The route pattern.</param>
        /// <param name="delimiter">The next middleware.</param>
        protected CsvRoutingMiddlewareBase(string routePattern, RequestDelegate next, char delimiter = ',')
        {
            if (routePattern == null)
            {
                throw new ArgumentNullException(nameof(routePattern));
            }

            // ensure _routePattern starts with /
            _routePattern = routePattern.StartsWith('/') ? routePattern : $"/{routePattern}";
            Delimiter = delimiter;
            _escapingChars = new[] { Delimiter, '"', '\r', '\n', '\t', ' ' };
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        ///     Invoke the OData Route debug middleware.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>A task that can be awaited.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.Request;

            if (string.Equals(request.Path.Value, _routePattern, StringComparison.OrdinalIgnoreCase))
            {
                await WriteRoutesAsCsv(context).ConfigureAwait(false);
            }
            else
            {
                await _next(context).ConfigureAwait(false);
            }
        }

        private async Task WriteRoutesAsCsv(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var csvTable = GenerateCsv(context);

            context.Response.ContentType = "text/csv";
            await context.Response.WriteAsync(csvTable).ConfigureAwait(false);
        }

        protected abstract string GenerateCsv(HttpContext context);

        protected string GetCsvValue(string? value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (_escapingChars.Intersect(value).Any())
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }
    }
}