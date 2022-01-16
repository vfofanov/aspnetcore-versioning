namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public class EdmModelOperationExtractorFactory : IEdmModelOperationExtractorFactory
    {
        public IEdmModelOperationExtractor Create(IEdmModelBuilderContext context)
        {
            return new EdmModelOperationExtractor(context);
        }
    }
}