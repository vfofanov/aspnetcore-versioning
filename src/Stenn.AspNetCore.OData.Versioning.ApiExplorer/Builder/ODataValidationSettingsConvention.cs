using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.OData.ModelBuilder.Config;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Represents an OData query options convention based on <see cref="ODataValidationSettings">validation settings</see>
    ///     .
    /// </summary>
    public class ODataValidationSettingsConvention : IODataQueryOptionsConvention
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ODataValidationSettingsConvention" /> class.
        /// </summary>
        /// <param name="validationSettings">
        ///     The <see cref="ODataValidationSettings">validation settings</see> the convention is
        ///     based on.
        /// </param>
        /// <param name="settings">The <see cref="ODataQueryOptionSettings">settings</see> used by the convention.</param>
        public ODataValidationSettingsConvention(ODataValidationSettings validationSettings, ODataQueryOptionSettings settings)
        {
            ValidationSettings = validationSettings;
            Settings = settings;
        }

        /// <summary>
        ///     Gets the validation settings used for the query options convention.
        /// </summary>
        /// <value>The <see cref="ODataValidationSettings">validation settings</see> for the convention.</value>
        protected ODataValidationSettings ValidationSettings { get; }

        /// <summary>
        ///     Gets the settings for OData query options.
        /// </summary>
        /// <value>The <see cref="ODataQueryOptionSettings">settings</see> used by the convention.</value>
        protected ODataQueryOptionSettings Settings { get; }

        /// <inheritdoc />
        public virtual void ApplyTo(ApiDescription apiDescription)
        {
            if (apiDescription == null)
            {
                throw new ArgumentNullException(nameof(apiDescription));
            }

            if (!IsSupported(apiDescription.HttpMethod))
            {
                return;
            }

            var context = new ODataQueryOptionDescriptionContext(ValidationSettings);
            var model = apiDescription.EdmModel();
            var queryOptions = GetQueryOptions(Settings.DefaultQuerySettings!, context);
            var singleResult = IsSingleResult(apiDescription, out var resultType);
            var visitor = new ODataAttributeVisitor(context, model, queryOptions, resultType, singleResult);

            var queryEnabled = visitor.Visit(apiDescription);
            if (!queryEnabled)
            {
                return;
            }

            var options = visitor.AllowedQueryOptions;
            var parameterDescriptions = apiDescription.ParameterDescriptions;

            if (options.HasFlag(AllowedQueryOptions.Select))
            {
                parameterDescriptions.Add(NewSelectParameter(context));
            }

            if (options.HasFlag(AllowedQueryOptions.Expand))
            {
                parameterDescriptions.Add(NewExpandParameter(context));
            }

            if (singleResult)
            {
                return;
            }

            if (options.HasFlag(AllowedQueryOptions.Filter))
            {
                parameterDescriptions.Add(NewFilterParameter(context));
            }

            if (options.HasFlag(AllowedQueryOptions.OrderBy))
            {
                parameterDescriptions.Add(NewOrderByParameter(context));
            }

            if (options.HasFlag(AllowedQueryOptions.Top))
            {
                parameterDescriptions.Add(NewTopParameter(context));
            }

            if (options.HasFlag(AllowedQueryOptions.Skip))
            {
                parameterDescriptions.Add(NewSkipParameter(context));
            }

            if (options.HasFlag(AllowedQueryOptions.Count))
            {
                parameterDescriptions.Add(NewCountParameter(context));
            }
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $filter query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewFilterParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.Filter, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.Filter), description, typeof(string));
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $expand query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewExpandParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.Expand, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.Expand), description, typeof(string));
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $select query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewSelectParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.Select, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.Select), description, typeof(string));
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $orderby query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewOrderByParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.OrderBy, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.OrderBy), description, typeof(string));
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $top query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewTopParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.Top, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.Top), description, typeof(int));
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $skip query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewSkipParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.Skip, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.Skip), description, typeof(int));
        }

        /// <summary>
        ///     Creates and returns a new parameter descriptor for the $count query option.
        /// </summary>
        /// <param name="descriptionContext">
        ///     The <see cref="ODataQueryOptionDescriptionContext">validation settings</see> used to
        ///     create the parameter.
        /// </param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewCountParameter(ODataQueryOptionDescriptionContext descriptionContext)
        {
            var description = Settings.DescriptionProvider.Describe(AllowedQueryOptions.Count, descriptionContext);
            return NewParameterDescription(GetName(AllowedQueryOptions.Count), description, typeof(bool), false);
        }

        // REF: http://docs.oasis-open.org/odata/odata/v4.01/cs01/part2-url-conventions/odata-v4.01-cs01-part2-url-conventions.html#sec_SystemQueryOptions
        private static bool IsSupported(string? httpMethod)
        {
            return httpMethod?.ToUpperInvariant() switch
            {
                "GET" => true,
                "PUT" => true,
                "PATCH" => true,
                "POST" => true,
                _ => false
            };
        }

        private string GetName(AllowedQueryOptions option)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            var name = option.ToString().ToLowerInvariant();
#pragma warning restore CA1308
            return Settings.NoDollarPrefix ? name : name.Insert(0, "$");
        }

        private AllowedQueryOptions GetQueryOptions(DefaultQuerySettings settings, ODataQueryOptionDescriptionContext context)
        {
            var queryOptions = ValidationSettings.AllowedQueryOptions;

            if (settings.EnableCount)
            {
                queryOptions |= AllowedQueryOptions.Count;
            }

            if (settings.EnableExpand)
            {
                queryOptions |= AllowedQueryOptions.Expand;
            }

            if (settings.EnableFilter)
            {
                queryOptions |= AllowedQueryOptions.Filter;
            }

            if (settings.EnableOrderBy)
            {
                queryOptions |= AllowedQueryOptions.OrderBy;
            }

            if (settings.EnableSelect)
            {
                queryOptions |= AllowedQueryOptions.Select;
            }

            if (settings.MaxTop is > 0)
            {
                context.MaxTop = settings.MaxTop;
            }

            return queryOptions;
        }

        /// <summary>
        ///     Creates a new API parameter description.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="description">The parameter description.</param>
        /// <param name="type">The parameter value type.</param>
        /// <param name="defaultValue">The parameter default value, if any.</param>
        /// <returns>A new <see cref="ApiParameterDescription">parameter description</see>.</returns>
        protected virtual ApiParameterDescription NewParameterDescription(string name, string description, Type type, object? defaultValue = default)
        {
            return new ApiParameterDescription
            {
                DefaultValue = defaultValue,
                IsRequired = false,
                ModelMetadata = new ODataQueryOptionModelMetadata(Settings.ModelMetadataProvider!, type, description),
                Name = name,
                ParameterDescriptor = new ParameterDescriptor
                {
                    Name = name,
                    ParameterType = type
                },
                Source = BindingSource.Query,
                Type = type
            };
        }

        private static bool IsSingleResult(ApiDescription description, out Type? resultType)
        {
            if (description.SupportedResponseTypes.Count == 0)
            {
                resultType = default;
                return true;
            }

            var supportedResponseTypes = description.SupportedResponseTypes;
            var candidates = new List<ApiResponseType>(supportedResponseTypes.Count);

            for (var i = 0; i < supportedResponseTypes.Count; i++)
            {
                var supportedResponseType = supportedResponseTypes[i];

                if (supportedResponseType.Type == null)
                {
                    continue;
                }

                var statusCode = supportedResponseType.StatusCode;

                if (statusCode is >= StatusCodes.Status200OK and < StatusCodes.Status300MultipleChoices)
                {
                    candidates.Add(supportedResponseType);
                }
            }

            if (candidates.Count == 0)
            {
                resultType = default;
                return true;
            }

            candidates.Sort((r1, r2) => r1.StatusCode.CompareTo(r2.StatusCode));

            var responseType = candidates[0].Type?.ExtractInnerType();

            if (responseType?.IsEnumerable(out resultType) == true)
            {
                resultType = null;
                return false;
            }
            resultType = responseType;
            return true;
        }
    }
}