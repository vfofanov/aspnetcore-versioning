using System;
using Microsoft.AspNetCore.Http;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///     Provides extension methods for the <see cref="HttpContext" />.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        ///     Return the <see cref="IODataAuthorizedFeature" /> from the <see cref="HttpContext" />.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext" /> instance to extend.</param>
        /// <param name="statusCode"></param>
        /// <returns>The <see cref="IODataAuthorizedFeature" />.</returns>
        public static IODataAuthorizedFeature AddODataAuthorizedFeature(this HttpContext httpContext, int? statusCode = null)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var feature = httpContext.Features.Get<IODataAuthorizedFeature?>();
            if (feature == null)
            {
                feature = new ODataAuthorizedFeature();
                httpContext.Features.Set(feature);
            }
            if (statusCode != null)
            {
                feature.StatusCode = statusCode.Value;
            }
            return feature;
        }

        /// <summary>
        ///     Return the <see cref="IODataAuthorizedFeature" /> from the <see cref="HttpContext" />.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext" /> instance to extend.</param>
        /// <returns>The <see cref="IODataAuthorizedFeature" />.</returns>
        public static IODataAuthorizedFeature? GetODataAuthorizedFeature(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            return httpContext.Features.Get<IODataAuthorizedFeature?>();
        }
    }
}