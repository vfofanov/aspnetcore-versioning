using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Default api version provider factory for use nominal versioning.
    /// If you are planning use versioning in future
    /// It will use 'v1.0' placeholder in routing
    /// </summary>
    public sealed class DefaultApiVersionInfoProviderFactory : IApiVersionInfoProviderFactory
    {
        /// <inheritdoc />
        public IApiVersionInfoProvider Create()
        {
            var version = new ApiVersion(1, 0);
            return new ApiVersionInfoProvider(version, new ApiVersionInfo(version, "v1.0"));
        }
    }
}