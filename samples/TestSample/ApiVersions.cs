using Microsoft.AspNetCore.Mvc;
using Stenn.AspNetCore.Versioning;

namespace TestSample
{
    /// <summary>
    /// Contains logic for control api versioning
    /// </summary>
    public static class ApiVersions
    {
        public static readonly ApiVersion V1 = new(1, 0);
        public static readonly ApiVersion V2 = new(2, 0);
        public static readonly ApiVersion V3 = new(3, 0);
    }

    public sealed class ApiVersionV1Attribute : ApiVersionAttribute
    {
        /// <inheritdoc />
        public ApiVersionV1Attribute()
            : base(ApiVersions.V1)
        {
        }
    }

    public sealed class ApiVersionV2Attribute : ApiVersionAttribute
    {
        /// <inheritdoc />
        public ApiVersionV2Attribute()
            : base(ApiVersions.V2)
        {
        }
    }
    
    public sealed class ApiVersionV3Attribute : ApiVersionAttribute
    {
        /// <inheritdoc />
        public ApiVersionV3Attribute()
            : base(ApiVersions.V3)
        {
        }
    }

    public sealed class ApiVersionInfoProviderFactory : IApiVersionInfoProviderFactory
    {
        /// <inheritdoc />
        public IApiVersionInfoProvider Create()
        {
            const string pathPartFormat = "v{0}";

            return new ApiVersionInfoProvider(ApiVersions.V2,
                new ApiVersionInfo(ApiVersions.V3, string.Format(pathPartFormat, ApiVersions.V3)),
                new ApiVersionInfo(ApiVersions.V2, string.Format(pathPartFormat, ApiVersions.V2)),
                new ApiVersionInfo(ApiVersions.V1, string.Format(pathPartFormat, ApiVersions.V1)));
        }
    }
}