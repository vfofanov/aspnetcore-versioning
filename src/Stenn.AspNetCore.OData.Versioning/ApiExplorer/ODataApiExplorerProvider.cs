using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OData.ModelBuilder.Config;
using Microsoft.OData.UriParser;
using Stenn.AspNetCore.OData.Versioning;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer
{
    /// <summary>
    ///     Represents an API explorer that provides <see cref="ApiDescription">API descriptions</see> for actions' query
    ///     parameters represented by
    ///     <see cref="ControllerActionDescriptor">controller action descriptors</see> that are defined by
    ///     OData controllers and are <see cref="ApiVersion">API version</see> aware.
    /// </summary>
    public class ODataQueryParametersApiDescriptionProvider : IApiDescriptionProvider
    {
        private static readonly Regex ActionParametersRegex = new(@"\((\w|{).+?}\)\z",
            RegexOptions.Singleline | RegexOptions.RightToLeft | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex ActionParameterReplaceRegex = new(@"(?<left>\w.+?=)?{(?<p>.+?)}",
            RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private const int AfterApiVersioning = -100;
        private readonly IOptions<ODataOptions> _odataOptions;
        private readonly IOptions<ODataVersioningOptions> _odataVersioningOptions;

        public ODataQueryParametersApiDescriptionProvider(
            IModelMetadataProvider metadataProvider,
            IOptions<ODataVersioningOptions> odataVersioningOptions,
            IOptions<ODataOptions> odataOptions)
        {
            _odataOptions = odataOptions;
            MetadataProvider = metadataProvider;
            _odataVersioningOptions = odataVersioningOptions;
        }


        /// <summary>
        ///     Gets the model metadata provider associated with the description provider.
        /// </summary>
        /// <value>The <see cref="IModelMetadataProvider">provider</see> used to retrieve model metadata.</value>
        protected IModelMetadataProvider MetadataProvider { get; }

        protected DefaultQuerySettings DefaultQuerySettings => ODataOptions.QuerySettings;

        protected ODataOptions ODataOptions => _odataOptions.Value;

        protected ODataVersioningOptions ODataVersioningOptions => _odataVersioningOptions.Value;

        /// <summary>
        ///     Gets the order precedence of the current API description provider.
        /// </summary>
        /// <value>The order precedence of the current API description provider. The default value is -100.</value>
        public virtual int Order => AfterApiVersioning;

        /// <summary>
        ///     Occurs after the providers have been executed.
        /// </summary>
        /// <param name="context">The current <see cref="ApiDescriptionProviderContext">execution context</see>.</param>
        /// <remarks>The default implementation performs no action.</remarks>
        public virtual void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var results = context.Results;

            
            foreach (var apiDescription in results)
            {
                if (apiDescription.ActionDescriptor is not ControllerActionDescriptor action ||
                    !action.ControllerTypeInfo.IsODataController() ||
                    action.ControllerTypeInfo.IsMetadataController())
                {
                    continue;
                }

                ProcessActionsWithParameters(apiDescription);

                var oDataRoutingMetadata = ODataEndpointExtensions.GetODataRoutingMetadata(action.EndpointMetadata);
                if (oDataRoutingMetadata == null)
                {
                    continue;
                }
                var routeComponent = ODataOptions.RouteComponents[oDataRoutingMetadata.Prefix];
                var serviceProvider = routeComponent.ServiceProvider;
                var odataUriResolver = serviceProvider.GetRequiredService<ODataUriResolver>();
                ExploreQueryOptions(new List<ApiDescription> { apiDescription }, odataUriResolver);
            }

            // foreach (var action in context.Actions.OfType<ControllerActionDescriptor>())
            // {
            //     if (!action.ControllerTypeInfo.IsODataController() ||
            //         action.ControllerTypeInfo.IsMetadataController() ||
            //         !IsVisible(action))
            //     {
            //         continue;
            //     }
            //     
            //     for (var i = 0; i < ApiVersionInfoProvider.Versions.Count; i++)
            //     {
            //         var versionInfo = ApiVersionInfoProvider.Versions[i];
            //
            //         // if (!IsMappedTo(action, versionInfo))
            //         // {
            //         //     continue;
            //         // }
            //         //
            //         // var mappedVersions = versionInfo.ModelSelector.ApiVersions;
            //         //
            //         // for (var j = 0; j < mappedVersions.Count; j++)
            //         // {
            //         //     var apiVersion = mappedVersions[j];
            //         //
            //         //     if (!action.IsMappedTo(apiVersion))
            //         //     {
            //         //         continue;
            //         //     }
            //         //
            //         //     var groupName = apiVersion.ToString(groupNameFormat, formatProvider);
            //         //     var descriptions = new List<ApiDescription>();
            //         //
            //         //     // foreach (var apiDescription in NewODataApiDescriptions(action, apiVersion, groupName, versionInfo))
            //         //     // {
            //         //     //     results.Add(apiDescription);
            //         //     //     descriptions.Add(apiDescription);
            //         //     // }
            //         //
            //         //     if (descriptions.Count > 0)
            //         //     {
            //         //         ExploreQueryOptions(descriptions, ODataUriResolver);// versionInfo.Services.GetRequiredService<ODataUriResolver>());
            //         //     }
            //         // }
            //     }
            // }
        }

        /// <summary>
        /// Replace path parameters ({p1},{p2}) to query parameters (p1=@p1,p2=@p2)?@p1=..&amp;@p2=..
        /// </summary>
        /// <param name="apiDescription"></param>
        private static void ProcessActionsWithParameters(ApiDescription apiDescription)
        {
            apiDescription.RelativePath = ActionParametersRegex.Replace(apiDescription.RelativePath,
                match =>
                {
                    return ActionParameterReplaceRegex.Replace(match.Value, paramMatch =>
                    {
                        var paramName = paramMatch.Groups["p"].Value;

                        var param = apiDescription.ParameterDescriptions.First(p => p.Name == paramName);
                        param.Name = $"@{paramName}";
                        param.Source = BindingSource.Query;
                        return $"{paramName}={param.Name}";
                    });
                });
        }

        /// <summary>
        ///     Occurs when the providers are being executed.
        /// </summary>
        /// <param name="context">The current <see cref="ApiDescriptionProviderContext">execution context</see>.</param>
        /// <remarks>The default implementation performs no operation.</remarks>
        public virtual void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
        }

        // /// <summary>
        // /// Returns a value indicating whether the specified action is visible to the API Explorer.
        // /// </summary>
        // /// <param name="action">The <see cref="ActionDescriptor">action</see> to evaluate.</param>
        // /// <returns>True if the <paramref name="action"/> is visible; otherwise, false.</returns>
        // protected bool IsVisible(ActionDescriptor action)
        // {
        //     return action.GetProperty<ApiExplorerModel>()?.IsVisible ?? false;
        // }

        /// <summary>
        ///     Explores the OData query options for the specified API descriptions.
        /// </summary>
        /// <param name="apiDescriptions">The <see cref="ApiDescription">API descriptions</see> to explore.</param>
        /// <param name="uriResolver">The associated <see cref="ODataUriResolver">OData URI resolver</see>.</param>
        protected virtual void ExploreQueryOptions(IEnumerable<ApiDescription> apiDescriptions, ODataUriResolver uriResolver)
        {
            if (uriResolver == null)
            {
                throw new ArgumentNullException(nameof(uriResolver));
            }

            var queryOptions = ODataVersioningOptions.ODataQueryOptions;
            //queryOptions.DescriptionProvider = new DefaultODataQueryOptionDescriptionProvider();


            var settings = new ODataQueryOptionSettings
            {
                NoDollarPrefix = uriResolver.EnableNoDollarQueryOptions,
                DescriptionProvider = queryOptions.DescriptionProvider,
                DefaultQuerySettings = DefaultQuerySettings,
                ModelMetadataProvider = MetadataProvider
            };

            queryOptions.ApplyTo(apiDescriptions, settings);
        }
    }
}