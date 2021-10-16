using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Api versions provider. Stores information about all api versions 
    /// </summary>
    public class ApiVersionInfoProvider : IApiVersionInfoProvider
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="defaultVersion"></param>
        /// <param name="versions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ApiVersionInfoProvider(ApiVersion defaultVersion, params ApiVersionInfo[] versions)
        {
            if (versions == null)
            {
                throw new ArgumentNullException(nameof(versions));
            }
            if (versions.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(versions));
            }
            Default = versions.Single(v => v.Version == defaultVersion);


            Versions = versions;
        }
        
        public ApiVersionInfo Default { get; }

        public IReadOnlyList<ApiVersionInfo> Versions { get; }
    }
}