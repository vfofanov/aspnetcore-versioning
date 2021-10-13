using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.Versioning;

namespace Microsoft.AspNet.OData
{
    /// <summary>
    ///     Represents a type substitution context.
    /// </summary>
    public class TypeSubstitutionContext
    {
        private readonly IServiceProvider? _serviceProvider;
        private ApiVersion? _apiVersion;
        private IEdmModel? _model;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeSubstitutionContext" /> class.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel">EDM model</see> to compare against.</param>
        /// <param name="modelTypeBuilder">The associated <see cref="IModelTypeBuilder">model type builder</see>.</param>
        public TypeSubstitutionContext(IEdmModel model, IModelTypeBuilder modelTypeBuilder)
        {
            this._model = model;
            ModelTypeBuilder = modelTypeBuilder;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeSubstitutionContext" /> class.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The <see cref="IServiceProvider">service provider</see> that the
        ///     <see cref="IEdmModel">EDM model</see> can be resolved from.
        /// </param>
        /// <param name="modelTypeBuilder">The associated <see cref="IModelTypeBuilder">model type builder</see>.</param>
        /// <param name="apiVersion">The current <see cref="ApiVersion">API version</see>.</param>
        public TypeSubstitutionContext(IServiceProvider serviceProvider, IModelTypeBuilder modelTypeBuilder, ApiVersion apiVersion)
        {
            this._apiVersion = apiVersion;
            this._serviceProvider = serviceProvider;
            ModelTypeBuilder = modelTypeBuilder;
        }

        /// <summary>
        ///     Gets the source Entity Data Model (EDM).
        /// </summary>
        /// <value>The associated <see cref="IEdmModel">EDM model</see> compared against for substitutions.</value>
        public IEdmModel Model => _model ??= _serviceProvider!.GetRequiredService<IEdmModelSelector>().SelectModel(_apiVersion)!;

        /// <summary>
        ///     Gets API version associated with the source model.
        /// </summary>
        /// <value>The associated <see cref="ApiVersion">API version</see>.</value>
        public ApiVersion ApiVersion => _apiVersion ??= _model!.GetAnnotationValue<ApiVersionAnnotation>(Model)?.ApiVersion ?? ApiVersion.Neutral;

        /// <summary>
        ///     Gets the model type builder used to create substitution types.
        /// </summary>
        /// <value>The associated <see cref="IModelTypeBuilder">model type builder</see>.</value>
        public IModelTypeBuilder ModelTypeBuilder { get; }
    }
}