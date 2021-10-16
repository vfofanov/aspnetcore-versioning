using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Represents an annotation for <see cref="ApiVersionInfo">API version</see>.
    /// </summary>
    [DebuggerDisplay("{Version}")]
    public class ApiVersionAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionAnnotation"/> class.
        /// </summary>
        /// <param name="info">The annotated <see cref="ApiVersionInfo">API version info</see>.</param>
        public ApiVersionAnnotation(ApiVersionInfo info) => Info = info;

        /// <summary>
        /// Gets the annotated API version info.
        /// </summary>
        /// <value>The annotated <see cref="Version">API version</see>.</value>
        public ApiVersionInfo Info { get; }

        /// <summary>
        /// Gets the annotated API version.
        /// </summary>
        /// <value>The annotated <see cref="Version">API version</see>.</value>
        public ApiVersion Version => Info.Version;
    }
}