using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IVersioningEdmModelFilter : IEdmModelFilter
    {
        /// <summary>
        /// Set current request <see cref="ApiVersion"/>
        /// </summary>
        void SetVersion(ApiVersion version);
    }
}