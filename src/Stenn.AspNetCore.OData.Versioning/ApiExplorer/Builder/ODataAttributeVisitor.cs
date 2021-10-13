using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.ModelBuilder.Config;

namespace Microsoft.AspNet.OData.Builder
{
    internal sealed class ODataAttributeVisitor
    {
        private readonly ODataQueryOptionDescriptionContext _context;
        private readonly IEdmModel? _model;
        private readonly Type? _resultType;
        private readonly StructuredTypeResolver _typeResolver;

        internal ODataAttributeVisitor(
            ODataQueryOptionDescriptionContext context,
            IEdmModel? model,
            AllowedQueryOptions allowedQueryOptions,
            Type? resultType,
            bool singleResult)
        {
            _context = context;
            AllowedQueryOptions = allowedQueryOptions;
            _resultType = resultType;
            IsSingleResult = singleResult;
            _model = model;
            _typeResolver = new StructuredTypeResolver(model);
        }

        internal AllowedQueryOptions AllowedQueryOptions { get; private set; }

        private bool IsSingleResult { get; }

        internal void Visit(ApiDescription apiDescription)
        {
            VisitAction(apiDescription.ActionDescriptor);

            if (_resultType == null)
            {
                return;
            }

            var modelType = _typeResolver.GetStructuredType(_resultType);

            if (modelType != null)
            {
                VisitModel(modelType);
            }
        }

        private void VisitModel(IEdmStructuredType modelType)
        {
            var querySettings = _model.GetAnnotationValue<ModelBoundQuerySettings>(modelType);

            if (querySettings == null)
            {
                return;
            }

            var properties = new HashSet<string>(modelType.Properties().Select(p => p.Name), StringComparer.OrdinalIgnoreCase);

            VisitSelect(querySettings, properties);
            VisitExpand(querySettings, properties);

            if (IsSingleResult)
            {
                return;
            }

            VisitCount(querySettings);
            VisitFilter(querySettings, properties);
            VisitOrderBy(querySettings, properties);
            VisitMaxTop(querySettings);
        }

        private void VisitEnableQuery(IReadOnlyList<EnableQueryAttribute> attributes)
        {
            var @default = new EnableQueryAttribute();

            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];

                if (attribute.AllowedArithmeticOperators == AllowedArithmeticOperators.None)
                {
                    _context.AllowedArithmeticOperators = AllowedArithmeticOperators.None;
                }
                else
                {
                    _context.AllowedArithmeticOperators |= attribute.AllowedArithmeticOperators;
                }

                if (attribute.AllowedFunctions == AllowedFunctions.None)
                {
                    _context.AllowedFunctions = AllowedFunctions.None;
                }
                else
                {
                    _context.AllowedFunctions |= attribute.AllowedFunctions;
                }

                if (attribute.AllowedLogicalOperators == AllowedLogicalOperators.None)
                {
                    _context.AllowedLogicalOperators = AllowedLogicalOperators.None;
                }
                else
                {
                    _context.AllowedLogicalOperators |= attribute.AllowedLogicalOperators;
                }

                if (attribute.AllowedQueryOptions == AllowedQueryOptions.None)
                {
                    AllowedQueryOptions = AllowedQueryOptions.None;
                }
                else
                {
                    AllowedQueryOptions |= attribute.AllowedQueryOptions;
                }

                if (_context.MaxAnyAllExpressionDepth == @default.MaxAnyAllExpressionDepth)
                {
                    _context.MaxAnyAllExpressionDepth = attribute.MaxAnyAllExpressionDepth;
                }

                if (_context.MaxExpansionDepth == @default.MaxExpansionDepth)
                {
                    _context.MaxExpansionDepth = attribute.MaxExpansionDepth;
                }

                if (_context.MaxNodeCount == @default.MaxNodeCount)
                {
                    _context.MaxNodeCount = attribute.MaxNodeCount;
                }

                if (_context.MaxOrderByNodeCount == @default.MaxOrderByNodeCount)
                {
                    _context.MaxOrderByNodeCount = attribute.MaxOrderByNodeCount;
                }

                if (_context.MaxSkip != @default.MaxSkip)
                {
                    _context.MaxSkip = attribute.MaxSkip;
                }

                if (_context.MaxTop != @default.MaxTop)
                {
                    _context.MaxTop = attribute.MaxTop;
                }

                if (!string.IsNullOrEmpty(attribute.AllowedOrderByProperties))
                {
                    var properties = attribute.AllowedOrderByProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var allowedOrderByProperties = _context.AllowedOrderByProperties;
                    var comparer = StringComparer.OrdinalIgnoreCase;

                    for (var j = 0; j < properties.Length; j++)
                    {
                        var property = properties[j].Trim();

                        if (!string.IsNullOrEmpty(property) && !allowedOrderByProperties.Contains(property, comparer))
                        {
                            allowedOrderByProperties.Add(property);
                        }
                    }
                }
            }
        }

        private void VisitSelect(ModelBoundQuerySettings querySettings, ICollection<string> properties)
        {
            Visit(querySettings, properties, AllowedQueryOptions.Select, IsSelectEnabled, _context.AllowedSelectProperties, querySettings.SelectConfigurations,
                s => s != SelectExpandType.Disabled);
        }

        private void VisitExpand(ModelBoundQuerySettings querySettings, ICollection<string> properties)
        {
            var @default = new ExpandConfiguration();

            bool IsExpandAllowed(ExpandConfiguration expand)
            {
                if (expand.ExpandType == SelectExpandType.Disabled)
                {
                    return false;
                }

                if (expand.MaxDepth != @default.MaxDepth)
                {
                    _context.MaxExpansionDepth = expand.MaxDepth;
                }

                return true;
            }

            Visit(querySettings, properties, AllowedQueryOptions.Expand, IsExpandEnabled, _context.AllowedExpandProperties, querySettings.ExpandConfigurations,
                IsExpandAllowed);
        }

        private void VisitFilter(ModelBoundQuerySettings querySettings, ICollection<string> properties)
        {
            Visit(querySettings, properties, AllowedQueryOptions.Filter, IsFilterEnabled, _context.AllowedFilterProperties, querySettings.FilterConfigurations,
                s => s);
        }

        private void VisitOrderBy(ModelBoundQuerySettings querySettings, ICollection<string> properties)
        {
            Visit(querySettings, properties, AllowedQueryOptions.OrderBy, IsOrderByEnabled, _context.AllowedOrderByProperties,
                querySettings.OrderByConfigurations, s => s);
        }

        private void VisitCount(ModelBoundQuerySettings querySettings)
        {
            if (!querySettings.Countable.HasValue)
            {
                return;
            }

            if (querySettings.Countable.Value)
            {
                AllowedQueryOptions |= AllowedQueryOptions.Count;
            }
            else
            {
                AllowedQueryOptions &= ~AllowedQueryOptions.Count;
            }
        }

        private void VisitMaxTop(ModelBoundQuerySettings querySettings)
        {
            if (querySettings.MaxTop != null && querySettings.MaxTop.Value > 0)
            {
                _context.MaxTop = querySettings.MaxTop;
            }
        }

        private void Visit<TSetting>(
            ModelBoundQuerySettings querySettings,
            ICollection<string> properties,
            AllowedQueryOptions option,
            Func<ModelBoundQuerySettings, bool> enabled,
            IList<string> queryableProperties,
            Dictionary<string, TSetting> configurations,
            Func<TSetting, bool> allowed)
        {
            if (!enabled(querySettings))
            {
                AllowedQueryOptions &= ~option;
                queryableProperties.Clear();
                return;
            }

            AllowedQueryOptions |= option;

            if (configurations.Count == 0)
            {
                // skip property-specific configurations; everything is allowed
                return;
            }

            var comparer = StringComparer.OrdinalIgnoreCase;
            var allowedProperties = new HashSet<string>(comparer);
            var disallowedProperties = new HashSet<string>(comparer);

            foreach (var property in configurations)
            {
                var name = property.Key;

                // note: remember that model bound attributes might be using hard-coded attributes. we need
                // to account for a substituted type on a down-level model where the property does not exist
                if (!properties.Contains(name))
                {
                    continue;
                }

                if (allowed(property.Value))
                {
                    allowedProperties.Add(name);
                }
                else
                {
                    disallowedProperties.Add(name);
                }
            }

            // if there's no specifically allowed properties, allow them all
            if (allowedProperties.Count == 0)
            {
                foreach (var property in properties)
                {
                    allowedProperties.Add(property);
                }
            }

            // remove any disallowed properties
            allowedProperties.ExceptWith(disallowedProperties);

            // if the final allowed set results in all properties, then clear the
            // properties to keep message less verbose
            if (allowedProperties.Count == properties.Count)
            {
                queryableProperties.Clear();
                return;
            }

            foreach (var property in allowedProperties)
            {
                if (!queryableProperties.Contains(property, comparer))
                {
                    queryableProperties.Add(property);
                }
            }
        }

        private bool IsSelectEnabled(ModelBoundQuerySettings querySettings)
        {
            if (!querySettings.DefaultSelectType.HasValue)
            {
                return AllowedQueryOptions.HasFlag(AllowedQueryOptions.Select) ||
                       querySettings.SelectConfigurations.Any(p => p.Value != SelectExpandType.Disabled);
            }

            return querySettings.DefaultSelectType.Value != SelectExpandType.Disabled ||
                   querySettings.SelectConfigurations.Any(p => p.Value != SelectExpandType.Disabled);
        }

        private bool IsExpandEnabled(ModelBoundQuerySettings querySettings)
        {
            if (!querySettings.DefaultExpandType.HasValue)
            {
                return AllowedQueryOptions.HasFlag(AllowedQueryOptions.Expand) ||
                       querySettings.ExpandConfigurations.Any(p => p.Value.ExpandType != SelectExpandType.Disabled);
            }

            return querySettings.DefaultExpandType.Value != SelectExpandType.Disabled ||
                   querySettings.ExpandConfigurations.Any(p => p.Value.ExpandType != SelectExpandType.Disabled);
        }

        private bool IsFilterEnabled(ModelBoundQuerySettings querySettings)
        {
            if (!querySettings.DefaultEnableFilter.HasValue)
            {
                return AllowedQueryOptions.HasFlag(AllowedQueryOptions.Filter) ||
                       querySettings.FilterConfigurations.Any(p => p.Value);
            }

            return querySettings.DefaultEnableFilter.Value ||
                   querySettings.FilterConfigurations.Any(p => p.Value);
        }

        private bool IsOrderByEnabled(ModelBoundQuerySettings querySettings)
        {
            if (!querySettings.DefaultEnableOrderBy.HasValue)
            {
                return AllowedQueryOptions.HasFlag(AllowedQueryOptions.OrderBy) ||
                       querySettings.OrderByConfigurations.Any(p => p.Value);
            }

            return querySettings.DefaultEnableOrderBy.Value ||
                   querySettings.OrderByConfigurations.Any(p => p.Value);
        }

        private void VisitAction(ActionDescriptor action)
        {
            if (action is not ControllerActionDescriptor controllerAction)
            {
                return;
            }

            var attributes = new List<EnableQueryAttribute>(controllerAction.ControllerTypeInfo.GetCustomAttributes<EnableQueryAttribute>(true));

            attributes.AddRange(controllerAction.MethodInfo.GetCustomAttributes<EnableQueryAttribute>(true));
            VisitEnableQuery(attributes);
        }
    }
}