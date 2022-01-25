using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    public interface IParameterConfigurationInitializer
    {
        void Initialize(ParameterConfiguration configuration);
    }
}