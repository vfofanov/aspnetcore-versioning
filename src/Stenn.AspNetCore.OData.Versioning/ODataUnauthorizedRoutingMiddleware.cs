using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Stenn.AspNetCore.OData.Versioning
{
    public class ODataUnauthorizedRoutingMiddleware
    {
        private readonly RequestDelegate _next;

        public ODataUnauthorizedRoutingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var feature = context.GetODataAuthorizedFeature();
            if (feature != null)
            {
                context.Response.StatusCode = feature.StatusCode;
                await context.Response.CompleteAsync();
            }
            else
            {
                await _next(context);
            }
        }
    }
}