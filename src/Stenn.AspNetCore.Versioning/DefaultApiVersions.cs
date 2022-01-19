using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    public static class DefaultApiVersions
    {
        public sealed class Version1_0 : ApiVersionAttribute
        {
            public Version1_0() : base(V1_0)
            {
            }
        }

        public static ApiVersion V1_0 { get; } = new ApiVersion(1, 0);

        public static readonly ApiVersion[] Active =
        {
            V1_0
        };
    }
}