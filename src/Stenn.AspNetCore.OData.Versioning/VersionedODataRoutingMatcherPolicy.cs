// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;

namespace Stenn.AspNetCore.OData.Versioning
{
    public class VersionedODataRoutingMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        private readonly IODataTemplateTranslator _translator;
        private readonly IODataModelProvider _provider;
        private readonly ODataOptions _options;

        public VersionedODataRoutingMatcherPolicy(IODataTemplateTranslator translator,
            IODataModelProvider provider,
            IOptions<ODataOptions> options)
        {
            _translator = translator;
            _provider = provider;
            _options = options.Value;
        }

        public override int Order => 900 - 1; // minus 1 to make sure it's running before built-in OData matcher policy

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            return endpoints.Any(e => e.Metadata.OfType<ODataRoutingMetadata>().FirstOrDefault() != null);
        }

        /// <inheritdoc />
        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            
            var odataFeature = httpContext.ODataFeature();
            if (odataFeature.Path != null)
            {
                // If we have the OData path setting, it means there's some Policy working.
                // Let's skip this default OData matcher policy.
                return Task.CompletedTask;
            }

            for (var i = 0; i < candidates.Count; i++)
            {
                ref var candidate = ref candidates[i];
                if (!candidates.IsValidCandidate(i))
                {
                    continue;
                }

                var oDataRoutingMetadata = candidate.Endpoint.Metadata.OfType<IODataRoutingMetadata>().FirstOrDefault();
                if (oDataRoutingMetadata == null)
                {
                    continue;
                }

                var apiVersion = oDataRoutingMetadata.GetODataApiVersion();
                if (apiVersion == null || !candidate.Endpoint.IsODataApiVersionMatch(apiVersion))
                {
                    candidates.SetValidity(i, false);
                    continue;
                }

                var model = _provider.GetEdmModel(apiVersion, httpContext.RequestServices);
                if (model == null)
                {
                    candidates.SetValidity(i, false);
                    continue;
                }

                var translatorContext = new ODataTemplateTranslateContext(httpContext, candidate.Endpoint, candidate.Values, model);

                try
                {
                    var template = GetODataPathTemplate(oDataRoutingMetadata.Template, model);
                    
                    var odataPath = _translator.Translate(template, translatorContext);
                    if (odataPath != null)
                    {
                        odataFeature.RoutePrefix = oDataRoutingMetadata.Prefix;
                        odataFeature.Model = model;
                        odataFeature.Path = odataPath;

                        var options = new ODataOptions();
                        UpdateQuerySetting(options);
                        options.AddRouteComponents(model);
                        odataFeature.Services = options.GetRouteServices(string.Empty);

                        MergeRouteValues(translatorContext.UpdatedValues, candidate.Values);
                    }
                    else
                    {
                        candidates.SetValidity(i, false);
                    }
                }
                catch
                {
                    candidates.SetValidity(i, false);
                }
            }

            return Task.CompletedTask;
        }

        private static ODataPathTemplate GetODataPathTemplate(ODataPathTemplate metadataTemplate, IEdmModel model)
        {
            if (metadataTemplate == null)
            {
                return null;
            }
            if (metadataTemplate.Count == 0)
            {
                return metadataTemplate;
            }
            var segments = new List<ODataSegmentTemplate>(metadataTemplate.Count);
            foreach (var originSegment in metadataTemplate)
            {
                switch (originSegment)
                {
                    case EntitySetSegmentTemplate setSegmentTemplate:
                    {
                        var entitySet = model.FindDeclaredEntitySet(setSegmentTemplate.EntitySet.Name);
                        segments.Add(new EntitySetSegmentTemplate(entitySet));
                    }
                        break;
                    case KeySegmentTemplate keySegmentTemplate:
                    {
                        var navSource = model.FindDeclaredNavigationSource(keySegmentTemplate.NavigationSource.Name);
                        var type = navSource.EntityType();
                        segments.Add(CreateKeySegment(type, navSource));
                    }
                        break;
                    default:
                        segments.Add(originSegment);
                        break;
                }
            }
            
            return new ODataPathTemplate(segments);
        }

        //NOTE: Copied from KeySegmentTemplate due it internal ((
        //TODO: Run via reflection
        /// <summary>
        /// Create <see cref="KeySegmentTemplate"/> based on the given entity type and navigation source.
        /// </summary>
        /// <param name="entityType">The given entity type.</param>
        /// <param name="navigationSource">The given navigation source.</param>
        /// <param name="keyPrefix">The prefix used before key template.</param>
        /// <returns>The built <see cref="KeySegmentTemplate"/>.</returns>
        private static KeySegmentTemplate CreateKeySegment(IEdmEntityType entityType, IEdmNavigationSource navigationSource, string keyPrefix = "key")
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            IDictionary<string, string> keyTemplates = new Dictionary<string, string>();
            var keys = entityType.Key().ToArray();
            if (keys.Length == 1)
            {
                // Id={key}
                keyTemplates[keys[0].Name] = $"{{{keyPrefix}}}";
            }
            else
            {
                // Id1={keyId1},Id2={keyId2}
                foreach (var key in keys)
                {
                    keyTemplates[key.Name] = $"{{{keyPrefix}{key.Name}}}";
                }
            }

            return new KeySegmentTemplate(keyTemplates, entityType, navigationSource);
        }
        
        private void UpdateQuerySetting(ODataOptions options)
        {
            options.QuerySettings.EnableSelect = _options.QuerySettings.EnableSelect;
            options.QuerySettings.EnableCount = _options.QuerySettings.EnableCount;
            options.QuerySettings.EnableExpand = _options.QuerySettings.EnableExpand;
            options.QuerySettings.EnableFilter = _options.QuerySettings.EnableFilter;
            options.QuerySettings.EnableOrderBy = _options.QuerySettings.EnableOrderBy;
            options.QuerySettings.EnableSkipToken = _options.QuerySettings.EnableSkipToken;
            options.QuerySettings.MaxTop = _options.QuerySettings.MaxTop;
        }

        private static void MergeRouteValues(RouteValueDictionary updates, RouteValueDictionary source)
        {
            foreach (var data in updates)
            {
                source[data.Key] = data.Value;
            }
        }
    }
}
