using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public interface IEdmModelOperationExtractorFactory
    {
        IEdmModelOperationExtractor Create(IEdmModelBuilderContext context);
    }
}