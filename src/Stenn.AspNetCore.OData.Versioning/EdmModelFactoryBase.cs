#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning.Filters;
using Stenn.AspNetCore.OData.Versioning.Operations;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///  Base edm model factory
    /// </summary>
    public abstract class EdmModelFactoryBase : EdmModelFactoryBase<ApiVersion>
    {
        /// <inheritdoc />
        protected EdmModelFactoryBase(IEdmModelMutatorFactory modelMutatorFactory,
            IEdmModelOperationExtractorFactory operationExtractorFactory, string ns = "Default")
            : base(modelMutatorFactory, operationExtractorFactory, ns)
        {
        }

        /// <inheritdoc />
        public override ApiVersion GetKey(ApiVersion version)
        {
            return version;
        }
    }
    
    /// <summary>
    ///  Base edm model factory
    /// </summary>
    public abstract class EdmModelFactoryBase<TKey> : IEdmModelFactory<TKey>
    {
        protected EdmModelFactoryBase(IEdmModelMutatorFactory modelMutatorFactory,
            IEdmModelOperationExtractorFactory operationExtractorFactory,
            string ns = "Default")
        {
            Namespace = ns;
            MutatorFactory = modelMutatorFactory;
            OperationExtractorFactory = operationExtractorFactory;
        }

        private string Namespace { get; }
        private IEdmModelMutatorFactory MutatorFactory { get; }
        private IEdmModelOperationExtractorFactory OperationExtractorFactory { get; }

        /// <inheritdoc />
        public IEdmModel CreateModel(TKey modelKey, ApiVersion version, bool requestModel)
        {
            var builder = CreateBuilder();
            builder.Mutator = MutatorFactory.Create(builder.Builder, version, requestModel);
            builder.OperationExtractor = OperationExtractorFactory.Create(builder);
            builder.Namespace = Namespace;

            FillModel(builder, version, modelKey);

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

        /// <inheritdoc />
        public abstract TKey GetKey(ApiVersion version);

        protected abstract void FillModel(EdmModelBuilder builder, ApiVersion version, TKey modelKey);

        protected virtual EdmModelBuilder CreateBuilder()
        {
            return new EdmModelBuilder();
        }
    }
}