namespace Stenn.AspNetCore.Versioning
{
    public sealed class DefaultApiVersionInfoProviderFactory : IApiVersionInfoProviderFactory
    {
        /// <inheritdoc />
        public IApiVersionInfoProvider Create()
        {
            return new ApiVersionInfoProvider(DefaultApiVersions.V1_0,
                new ApiVersionInfo(DefaultApiVersions.V1_0, "v1.0"));
        }
    }
}