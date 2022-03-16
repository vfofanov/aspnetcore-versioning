using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer
{
    /// <summary>
    ///     Represents the model metadata for an OData query option.
    /// </summary>
    public sealed class ODataQueryOptionModelMetadata : ModelMetadata
    {
        private readonly ModelMetadata _inner;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ODataQueryOptionModelMetadata" /> class.
        /// </summary>
        /// <param name="modelMetadataProvider">
        ///     The <see cref="IModelMetadataProvider">model metadata provider</see>
        ///     used to create the new instance.
        /// </param>
        /// <param name="modelType">The type of OData query option.</param>
        /// <param name="description">The description associated with the model metadata.</param>
        public ODataQueryOptionModelMetadata(IModelMetadataProvider modelMetadataProvider, Type modelType, string description)
            : base(ModelMetadataIdentity.ForType(modelType))
        {
            if (modelMetadataProvider == null)
            {
                throw new ArgumentNullException(nameof(modelMetadataProvider));
            }

            _inner = modelMetadataProvider.GetMetadataForType(modelType);
            Description = description;
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<object, object> AdditionalValues => _inner.AdditionalValues;

        /// <inheritdoc />
        public override ModelPropertyCollection Properties => _inner.Properties;

        /// <inheritdoc />
        public override string? BinderModelName => _inner.BinderModelName;

        /// <inheritdoc />
        public override Type? BinderType => _inner.BinderType;

        /// <inheritdoc />
        public override BindingSource? BindingSource => _inner.BindingSource;

        /// <inheritdoc />
        public override bool ConvertEmptyStringToNull => _inner.ConvertEmptyStringToNull;

        /// <inheritdoc />
        public override string? DataTypeName => _inner.DataTypeName;

        /// <inheritdoc />
        public override string Description { get; }

        /// <inheritdoc />
        public override string? DisplayFormatString => _inner.DisplayFormatString;

        /// <inheritdoc />
#pragma warning disable CA1721 // Property names should not match get methods; inherited member
        public override string? DisplayName => _inner.DisplayName;
#pragma warning restore CA1721

        /// <inheritdoc />
        public override string? EditFormatString => _inner.EditFormatString;

        /// <inheritdoc />
        public override ModelMetadata? ElementMetadata => _inner.ElementMetadata;

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<EnumGroupAndName, string>>? EnumGroupedDisplayNamesAndValues => _inner.EnumGroupedDisplayNamesAndValues;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string>? EnumNamesAndValues => _inner.EnumNamesAndValues;

        /// <inheritdoc />
        public override bool HasNonDefaultEditFormat => _inner.HasNonDefaultEditFormat;

        /// <inheritdoc />
        public override bool HtmlEncode => _inner.HtmlEncode;

        /// <inheritdoc />
        public override bool HideSurroundingHtml => _inner.HideSurroundingHtml;

        /// <inheritdoc />
        public override bool IsBindingAllowed => _inner.IsBindingAllowed;

        /// <inheritdoc />
        public override bool IsBindingRequired => _inner.IsBindingRequired;

        /// <inheritdoc />
        public override bool IsEnum => _inner.IsEnum;

        /// <inheritdoc />
        public override bool IsFlagsEnum => _inner.IsFlagsEnum;

        /// <inheritdoc />
        public override bool IsReadOnly => _inner.IsReadOnly;

        /// <inheritdoc />
        public override bool IsRequired => _inner.IsRequired;

        /// <inheritdoc />
        public override ModelBindingMessageProvider ModelBindingMessageProvider => _inner.ModelBindingMessageProvider;

        /// <inheritdoc />
        public override int Order => _inner.Order;

        /// <inheritdoc />
        public override string? Placeholder => _inner.Placeholder;

        /// <inheritdoc />
        public override string? NullDisplayText => _inner.NullDisplayText;

        /// <inheritdoc />
        public override IPropertyFilterProvider? PropertyFilterProvider => _inner.PropertyFilterProvider;

        /// <inheritdoc />
        public override bool ShowForDisplay => _inner.ShowForDisplay;

        /// <inheritdoc />
        public override bool ShowForEdit => _inner.ShowForEdit;

        /// <inheritdoc />
        public override string? SimpleDisplayProperty => _inner.SimpleDisplayProperty;

        /// <inheritdoc />
        public override string? TemplateHint => _inner.TemplateHint;

        /// <inheritdoc />
        public override bool ValidateChildren => _inner.ValidateChildren;

        /// <inheritdoc />
        public override IReadOnlyList<object> ValidatorMetadata => _inner.ValidatorMetadata;

        /// <inheritdoc />
        public override Func<object, object?>? PropertyGetter => _inner.PropertyGetter;

        /// <inheritdoc />
        public override Action<object, object?>? PropertySetter => _inner.PropertySetter;
    }
}