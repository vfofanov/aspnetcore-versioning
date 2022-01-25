using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IEdmModelKeyHolder
    {
        /// <summary>
        /// Get unique <see cref="IEdmModel"/> model's key for holder
        /// </summary>
        /// <returns></returns>
        EdmModelKey GetKey();
    }
}