using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Represents an annotation for <see cref="ApiVersion">API version</see>.
    /// </summary>
    [DebuggerDisplay("{ApiVersion}")]
    public class ApiVersionAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionAnnotation"/> class.
        /// </summary>
        /// <param name="apiVersion">The annotated <see cref="ApiVersion">API version</see>.</param>
        public ApiVersionAnnotation(ApiVersion apiVersion) => ApiVersion = apiVersion;

        /// <summary>
        /// Gets the annotated API version.
        /// </summary>
        /// <value>The annotated <see cref="ApiVersion">API version</see>.</value>
        public ApiVersion ApiVersion { get; }
    }
}