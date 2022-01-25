using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    /// <summary>
    /// </summary>
    public interface IEdmModelMutatorFactory
    {
        IEdmModelMutator Create(ODataModelBuilder builder, ApiVersion version, bool requestModel);
    }
}