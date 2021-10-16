using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Api version provider item for store all information about api version
    /// </summary>
    [DebuggerDisplay("{Version}, Depricated:{IsDeprecated}")]
    public sealed class ApiVersionInfo : IEquatable<ApiVersionInfo>, IEquatable<ApiVersion>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="version"></param>
        /// <param name="versionApiName"></param>
        /// <param name="depricated"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiVersionInfo(ApiVersion version, string? versionApiName = null, bool depricated = false)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Version = version;
            IsDeprecated = depricated;
            RoutePathName = versionApiName ?? version.ToString();
            Annotation = new ApiVersionAnnotation(this);
        }

        /// <summary>
        /// Route path part for versioning and Api Explorer group name
        /// </summary>
        public string RoutePathName { get; }

        /// <summary>
        /// Api version
        /// </summary>
        public ApiVersion Version { get; }

        /// <summary>
        /// Annnotation for store it in <see cref="ControllerModel"/> and <see cref="ActionModel"/>
        /// </summary>
        public ApiVersionAnnotation Annotation { get; }

        public bool IsDeprecated { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns the text representation of the version using the specified format and format provider.
        /// <seealso cref="ApiVersionFormatProvider"/></summary>
        /// <param name="format">The format to return the text representation in. The value can be <c>null</c> or empty.</param>
        /// <returns>The <see cref="string">string</see> representation of the version.</returns>
        /// <exception cref="FormatException">The specified <paramref name="format"/> is not one of the supported format values.</exception>
        public string ToString(string format)
        {
            return Version.ToString(format);
        }

        /// <summary>
        /// Returns the text representation of the version using the specified format and format provider.
        /// <seealso cref="ApiVersionFormatProvider"/></summary>
        /// <param name="format">The format to return the text representation in. The value can be <c>null</c> or empty.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider">format provider</see> used to generate text.
        /// This implementation should typically use an <see cref="StringComparison.InvariantCulture">invariant culture</see>.</param>
        /// <returns>The <see cref="string">string</see> representation of the version.</returns>
        /// <exception cref="FormatException">The specified <paramref name="format"/> is not one of the supported format values.</exception>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return Version.ToString(format, formatProvider);
        }

        /// <inheritdoc />
        public bool Equals(ApiVersionInfo? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Version.Equals(other.Version);
        }

        /// <inheritdoc />
        public bool Equals(ApiVersion? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return Version.Equals(other);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj)
                   || obj is ApiVersionInfo other && Equals(other)
                   || obj is ApiVersion otherVersion && Equals(otherVersion);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }

        public static bool operator ==(ApiVersionInfo? left, ApiVersionInfo? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ApiVersionInfo? left, ApiVersionInfo? right)
        {
            return !Equals(left, right);
        }
    }
}