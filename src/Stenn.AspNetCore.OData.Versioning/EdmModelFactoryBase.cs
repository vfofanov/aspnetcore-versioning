#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///  Base edm model factory
    /// </summary>
    public abstract class EdmModelFactoryBase : IEdmModelFactory
    {
        protected EdmModelFactoryBase(string ns = "Default")
        {
            Namespace = ns;
        }

        private string Namespace { get; }
        public virtual EdmModelBuilder CreateBuilder()
        {
            return new EdmModelBuilder();
        }
        
        /// <inheritdoc />
        public IEdmModel CreateModel(EdmModelBuilder builder, ApiVersion version, bool requestModel)
        {
            builder.Namespace = Namespace;

            FillModel(builder, version);

            builder.Mutator.Run();

            builder.FinalizeBuilderIntenal();
            FinalizeBuilder(builder.Builder);

            return builder.Builder.GetEdmModel();
        }

        /// <summary>
        /// Final builder initialization after mutation, before generate model
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void FinalizeBuilder(ODataConventionModelBuilder builder)
        {
        }

        protected abstract void FillModel(EdmModelBuilder builder, ApiVersion version);
    }
}