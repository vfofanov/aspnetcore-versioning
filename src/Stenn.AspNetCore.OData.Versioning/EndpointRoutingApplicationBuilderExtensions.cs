using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    /// Constains extensions for configuring routing on an <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class EndpointRoutingApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="ODataUnauthorizedRoutingMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseODataUnauthorizedHandler(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            VerifyRoutingServicesAreRegistered(builder);
            return builder.UseMiddleware(typeof(ODataUnauthorizedRoutingMiddleware));
        }
        
        private static void VerifyRoutingServicesAreRegistered(IApplicationBuilder app)
        {
            // Verify if AddRouting was done before calling UseEndpointRouting/UseEndpoint
            // We use the RoutingMarkerService to make sure if all the services were added.
            if (app.ApplicationServices.GetService(typeof(ODataModelProvider)) == null)
            {
                throw new InvalidOperationException(
                    "Unable to find OData versioning services. Check that they added to services via VersioningAspNetCoreODataDependencyInjectionExtensions.AddVersioningOData");
            }
        }
    }
}