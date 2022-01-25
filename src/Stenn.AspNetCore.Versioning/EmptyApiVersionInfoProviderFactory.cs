using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    /// <summary>
    /// Empty api version provider factory for use nominal versioning.
    /// If you already have api without version, but you want use other features or you are planning use versioning in future
    /// It will use empty placeholder in routing
    /// </summary>
    public sealed class EmptyApiVersionInfoProviderFactory : IApiVersionInfoProviderFactory
    {
        /// <inheritdoc />
        public IApiVersionInfoProvider Create()
        {
            var version = new ApiVersion(0, 0);
            return new ApiVersionInfoProvider(version, new ApiVersionInfo(version, string.Empty));
        }
    }
}