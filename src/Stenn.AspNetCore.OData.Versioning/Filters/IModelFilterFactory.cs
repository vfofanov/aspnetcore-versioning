using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IModelFilterFactory
    {
        public IModelFilter Create(ApiVersion version);
    }
}