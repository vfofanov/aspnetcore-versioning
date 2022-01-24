using Microsoft.AspNetCore.Http;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    /// If this feature exists in <see cref="HttpContext.Features"/> this mean that this is OData unauthorized request
    /// </summary>
    public interface IODataAuthorizedFeature
    {
        int StatusCode { get; set; }
    }
}