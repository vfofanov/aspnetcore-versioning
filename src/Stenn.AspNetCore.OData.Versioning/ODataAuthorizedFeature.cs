using Microsoft.AspNetCore.Http;

namespace Stenn.AspNetCore.OData.Versioning
{
    public class ODataAuthorizedFeature : IODataAuthorizedFeature
    {
        /// <inheritdoc />
        public int StatusCode { get; set; } = StatusCodes.Status401Unauthorized;
    }
}