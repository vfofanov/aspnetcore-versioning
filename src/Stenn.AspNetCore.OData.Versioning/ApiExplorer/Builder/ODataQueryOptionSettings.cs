using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OData.ModelBuilder.Config;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Represents the settings for OData query options.
    /// </summary>
    public class ODataQueryOptionSettings
    {
        private IODataQueryOptionDescriptionProvider? _descriptionProvider;

        /// <summary>
        ///     Gets or sets a value indicating whether query options have the system "$" prefix.
        /// </summary>
        /// <value>
        ///     True if the OData query options use the "$" prefix; otherwise, false. The default
        ///     value is <c>false</c>.
        /// </value>
        public bool NoDollarPrefix { get; set; }

        /// <summary>
        ///     Gets or sets the provider used to describe query options.
        /// </summary>
        /// <value>The <see cref="IODataQueryOptionDescriptionProvider">provider</see> used to describe OData query options.</value>
        public IODataQueryOptionDescriptionProvider DescriptionProvider
        {
            get => _descriptionProvider ??= new DefaultODataQueryOptionDescriptionProvider();
            set => _descriptionProvider = value;
        }

        /// <summary>
        ///     Gets or sets the default OData query settings.
        /// </summary>
        /// <value>The <see cref="DefaultQuerySettings">default OData query settings</see>.</value>
        public DefaultQuerySettings? DefaultQuerySettings { get; set; }

        /// <summary>
        ///     Gets or sets the configured model metadata provider.
        /// </summary>
        /// <value>The configured <see cref="IModelMetadataProvider">model metadata provider</see>.</value>
        public IModelMetadataProvider? ModelMetadataProvider { get; set; }
    }
}