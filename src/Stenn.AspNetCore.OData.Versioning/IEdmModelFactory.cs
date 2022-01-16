#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IEdmModelFactory<TKey>
    {
        /// <summary>
        /// Get edm model key for specific api version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        TKey GetKey(ApiVersion version);

        /// <summary>
        /// Create edm model builder
        /// </summary>
        /// <param name="modelKey"></param>
        /// <param name="version"></param>
        /// <param name="requestModel"></param>
        IEdmModel CreateModel(TKey modelKey, ApiVersion version, bool requestModel);
    }
}