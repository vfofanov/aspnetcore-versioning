using Microsoft.Extensions.Options;
using Stenn.AspNetCore.OData.Versioning;

namespace TestSample.Controllers.OData
{
    /// <inheritdoc />
    public sealed class MetadataController : MetadataControllerBase
    {
        /// <inheritdoc />
        public MetadataController(IOptions<ODataVersioningOptions> options)
            : base(options)
        {
        }
    }
}