using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IEdmModelEntityType
    {
        EntityTypeConfiguration CommonType { get; }
    }
}