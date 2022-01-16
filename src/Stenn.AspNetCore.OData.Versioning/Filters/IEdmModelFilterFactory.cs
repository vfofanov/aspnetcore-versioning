using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IEdmModelFilterFactory
    {
        public IEdmModelFilter Create(ApiVersion version);
    }
}