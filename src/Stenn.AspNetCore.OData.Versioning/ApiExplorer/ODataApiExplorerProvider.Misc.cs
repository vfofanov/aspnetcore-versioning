using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Stenn.AspNetCore.Versioning;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer
{
    partial class ODataApiDescriptionProvider
    {
static bool IsMappedTo(ControllerActionDescriptor action, ODataRouteMapping mapping) =>
            action.AttributeRouteInfo is ODataAttributeRouteInfo routeInfo &&
            StringComparer.OrdinalIgnoreCase.Equals(routeInfo.RouteName, mapping.RouteName);

        static Type? GetDeclaredReturnType(ControllerActionDescriptor action)
        {
            var declaredReturnType = action.MethodInfo.ReturnType;

            if (declaredReturnType == typeof(void) || declaredReturnType == typeof(Task))
            {
                return typeof(void);
            }

            var unwrappedType = declaredReturnType;

            if (declaredReturnType.IsGenericType && declaredReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                unwrappedType = declaredReturnType.GetGenericArguments()[0];
            }

            if (typeof(IActionResult).IsAssignableFrom(unwrappedType))
            {
                return default;
            }

            return unwrappedType;
        }

        static Type? GetRuntimeReturnType(Type declaredReturnType) => declaredReturnType == typeof(object) ? default : declaredReturnType;

        static IReadOnlyList<IApiRequestMetadataProvider>? GetRequestMetadataAttributes(ControllerActionDescriptor action)
        {
            if (action.FilterDescriptors == null)
            {
                return default;
            }

            return action.FilterDescriptors.Select(fd => fd.Filter).OfType<IApiRequestMetadataProvider>().ToArray();
        }

        static IReadOnlyList<IApiResponseMetadataProvider>? GetResponseMetadataAttributes(ControllerActionDescriptor action)
        {
            if (action.FilterDescriptors == null)
            {
                return default;
            }

            return action.FilterDescriptors.Select(fd => fd.Filter).OfType<IApiResponseMetadataProvider>().ToArray();
        }

        static string? BuildRelativePath(ControllerActionDescriptor action, ODataRouteBuilderContext routeContext)
        {
            var relativePath = action.AttributeRouteInfo?.Template;

            if (!string.IsNullOrEmpty(relativePath) && routeContext.Options.UseQualifiedNames)
            {
                return relativePath;
            }

            var builder = new ODataRouteBuilder(routeContext);

            relativePath = builder.Build();

            if (builder.IsNavigationPropertyLink && action.AttributeRouteInfo is ODataAttributeRouteInfo info)
            {
                var template = info.ODataTemplate?.Segments.OfType<NavigationPropertyLinkSegmentTemplate>().FirstOrDefault();

                if (template == null)
                {
                    return relativePath;
                }

                var key = string.Concat("{", NavigationProperty, "}");
                var property = template.Segment.NavigationProperty;
                var value = property.Name;

                relativePath = relativePath.Replace(key, value, OrdinalIgnoreCase);

                if (action.ActionName.StartsWith("DeleteRef", Ordinal) &&
                    property.TargetMultiplicity() == EdmMultiplicity.Many)
                {
                    builder.AddOrReplaceRefIdQueryParameter();
                }
            }

            return relativePath;
        }

        IEnumerable<ApiDescription> NewODataApiDescriptions(
            ControllerActionDescriptor action,
            ApiVersion version,
            string groupName,
            ODataRouteMapping mapping)
        {
            var requestMetadataAttributes = GetRequestMetadataAttributes(action);
            var responseMetadataAttributes = GetResponseMetadataAttributes(action);
            var declaredReturnType = GetDeclaredReturnType(action);
            var runtimeReturnType = GetRuntimeReturnType(declaredReturnType!);
            var apiResponseTypes = GetApiResponseTypes(responseMetadataAttributes!, runtimeReturnType!, mapping.Services, version);
            var routeContext = new ODataRouteBuilderContext(mapping, version, action, Options) { ModelMetadataProvider = MetadataProvider };

            if (routeContext.IsRouteExcluded)
            {
                yield break;
            }

            var parameterContext = new ApiParameterContext(MetadataProvider, routeContext, ModelTypeBuilder);
            var parameters = GetParameters(parameterContext);

            for (var i = 0; i < parameters.Count; i++)
            {
                routeContext.ParameterDescriptions.Add(parameters[i]);
            }

            var relativePath = BuildRelativePath(action, routeContext);
            var apiExplorer = action.GetProperty<ApiExplorerModel>() ?? action.GetProperty<ControllerModel>()?.ApiExplorer;

            if (apiExplorer != null && !string.IsNullOrEmpty(apiExplorer.GroupName))
            {
                groupName = apiExplorer.GroupName;
            }

            parameters = routeContext.ParameterDescriptions;

            foreach (var httpMethod in action.GetHttpMethods())
            {
                var apiDescription = new ApiDescription()
                {
                    ActionDescriptor = action,
                    HttpMethod = httpMethod,
                    RelativePath = relativePath,
                    GroupName = groupName,
                    Properties = { [typeof(IEdmModel)] = routeContext.EdmModel },
                };

                if (routeContext.EntitySet != null)
                {
                    apiDescription.Properties[typeof(IEdmEntitySet)] = routeContext.EntitySet;
                }

                if (routeContext.Operation != null)
                {
                    apiDescription.Properties[typeof(IEdmOperation)] = routeContext.Operation;
                }

                for (var i = 0; i < parameters.Count; i++)
                {
                    apiDescription.ParameterDescriptions.Add(parameters[i]);
                }

                if (apiDescription.ParameterDescriptions.Count > 0)
                {
                    var contentTypes = GetDeclaredContentTypes(requestMetadataAttributes);

                    for (var i = 0; i < apiDescription.ParameterDescriptions.Count; i++)
                    {
                        var parameter = apiDescription.ParameterDescriptions[i];

                        if (parameter.Source == Body)
                        {
                            var requestFormats = GetSupportedFormats(contentTypes, parameter.Type);

                            for (var j = 0; j < requestFormats.Count; j++)
                            {
                                apiDescription.SupportedRequestFormats.Add(requestFormats[j]);
                            }
                        }
                        else if (parameter.Source == FormFile)
                        {
                            for (var j = 0; j < contentTypes.Count; j++)
                            {
                                apiDescription.SupportedRequestFormats.Add(new ApiRequestFormat() { MediaType = contentTypes[j] });
                            }
                        }
                    }
                }

                for (var i = 0; i < apiResponseTypes.Count; i++)
                {
                    apiDescription.SupportedResponseTypes.Add(apiResponseTypes[i]);
                }

                PopulateApiVersionParameters(apiDescription, version);
                apiDescription.SetApiVersion(version);
                apiDescription.TryUpdateRelativePathAndRemoveApiVersionParameter(Options);
                yield return apiDescription;
            }
        }

        IList<ApiParameterDescription> GetParameters(ApiParameterContext context)
        {
            var action = context.RouteContext.ActionDescriptor;

            if (action.Parameters != null)
            {
                for (var i = 0; i < action.Parameters.Count; i++)
                {
                    var actionParameter = action.Parameters[i];
                    var metadata = MetadataProvider.GetMetadataForType(actionParameter.ParameterType);

                    UpdateBindingInfo(context, actionParameter, metadata);

                    var visitor = new PseudoModelBindingVisitor(context, actionParameter);
                    var bindingContext = new ApiParameterDescriptionContext(metadata, actionParameter.BindingInfo, propertyName: actionParameter.Name);

                    visitor.WalkParameter(bindingContext);
                }
            }

            if (action.BoundProperties != null)
            {
                for (var i = 0; i < action.BoundProperties.Count; i++)
                {
                    var actionParameter = action.BoundProperties[i];
                    var visitor = new PseudoModelBindingVisitor(context, actionParameter);
                    var modelMetadata = context.MetadataProvider.GetMetadataForProperty(
                        containerType: action.ControllerTypeInfo.AsType(),
                        propertyName: actionParameter.Name);
                    var bindingContext = new ApiParameterDescriptionContext(
                        modelMetadata,
                        actionParameter.BindingInfo,
                        propertyName: actionParameter.Name);

                    visitor.WalkParameter(bindingContext);
                }
            }

            for (var i = context.Results.Count - 1; i >= 0; i--)
            {
                if (!context.Results[i].Source.IsFromRequest)
                {
                    context.Results.RemoveAt(i);
                }
            }

            ProcessRouteParameters(context);

            return context.Results;
        }

        static MediaTypeCollection GetDeclaredContentTypes(IReadOnlyList<IApiRequestMetadataProvider>? requestMetadataAttributes)
        {
            var contentTypes = new MediaTypeCollection();

            if (requestMetadataAttributes != null)
            {
                for (var i = 0; i < requestMetadataAttributes.Count; i++)
                {
                    requestMetadataAttributes[i].SetContentTypes(contentTypes);
                }
            }

            return contentTypes;
        }

        IReadOnlyList<ApiRequestFormat> GetSupportedFormats(MediaTypeCollection contentTypes, Type type)
        {
            if (contentTypes.Count == 0)
            {
                contentTypes = new MediaTypeCollection() { default(string) };
            }

            var results = new List<ApiRequestFormat>(capacity: contentTypes.Count);

            for (var i = 0; i < contentTypes.Count; i++)
            {
                for (var j = 0; j < MvcOptions.InputFormatters.Count; j++)
                {
                    var formatter = MvcOptions.InputFormatters[j];

                    if (!(formatter is IApiRequestFormatMetadataProvider requestFormatMetadataProvider))
                    {
                        continue;
                    }

                    IReadOnlyList<string>? supportedTypes;

                    try
                    {
                        supportedTypes = requestFormatMetadataProvider.GetSupportedContentTypes(contentTypes[i], type);
                    }
                    catch (InvalidOperationException)
                    {
                        // BUG: https://github.com/OData/WebApi/issues/1750
                        supportedTypes = null;
                    }

                    if (supportedTypes == null)
                    {
                        continue;
                    }

                    for (var k = 0; k < supportedTypes.Count; k++)
                    {
                        results.Add(new ApiRequestFormat() { Formatter = formatter, MediaType = supportedTypes[k], });
                    }
                }
            }

            return results.ToArray();
        }

        static void UpdateBindingInfo(ApiParameterContext context, ParameterDescriptor parameter, ModelMetadata metadata)
        {
            var parameterType = parameter.ParameterType;
            var bindingInfo = parameter.BindingInfo;

            if (bindingInfo == null)
            {
                parameter.BindingInfo = bindingInfo = new BindingInfo() { BindingSource = metadata.BindingSource };
            }
            else if (bindingInfo.BindingSource == null)
            {
                bindingInfo.BindingSource = metadata.BindingSource;
            }

            if (bindingInfo.BindingSource == Custom)
            {
                if (parameterType.IsODataQueryOptions() || parameterType.IsODataPath())
                {
                    bindingInfo.BindingSource = Special;
                }
            }

            if (bindingInfo.BindingSource != null)
            {
                return;
            }

            var key = default(IEdmNamedElement);
            var paramName = parameter.Name;
            var source = Query;

            switch (context.RouteContext.ActionType)
            {
                case EntitySet:
                    var keys = context.RouteContext.EntitySet.EntityType().Key().ToArray();

                    key = keys.FirstOrDefault(k => k.Name.Equals(paramName, OrdinalIgnoreCase));

                    if (key == null)
                    {
                        var template = context.PathTemplate;

                        if (template != null)
                        {
                            var segments = template.Segments.OfType<KeySegmentTemplate>();

                            if (segments.SelectMany(s => s.ParameterMappings.Values).Any(name => name.Equals(paramName, OrdinalIgnoreCase)))
                            {
                                source = Path;
                            }
                        }
                    }
                    else
                    {
                        source = Path;
                    }

                    break;
                case BoundOperation:
                case UnboundOperation:
                    var operation = context.RouteContext.Operation;

                    if (operation == null)
                    {
                        break;
                    }

                    key = operation.Parameters.FirstOrDefault(p => p.Name.Equals(paramName, OrdinalIgnoreCase));

                    if (key == null)
                    {
                        if (operation.IsBound)
                        {
                            goto case EntitySet;
                        }
                    }
                    else
                    {
                        source = Path;
                    }

                    break;
            }

            bindingInfo.BindingSource = source;
        }

        IReadOnlyList<ApiResponseType> GetApiResponseTypes(
            IReadOnlyList<IApiResponseMetadataProvider> responseMetadataAttributes,
            Type responseType,
            IServiceProvider serviceProvider,
            ApiVersion version)
        {
            var results = new List<ApiResponseType>();
            var objectTypes = new Dictionary<int, Type>();
            var contentTypes = new MediaTypeCollection();

            if (responseMetadataAttributes != null)
            {
                for (var i = 0; i < responseMetadataAttributes.Count; i++)
                {
                    var metadataAttribute = responseMetadataAttributes[i];

                    metadataAttribute.SetContentTypes(contentTypes);

                    var canInferResponseType = metadataAttribute.Type == typeof(void) &&
                                               responseType != null &&
                                               (metadataAttribute.StatusCode == Status200OK || metadataAttribute.StatusCode == Status201Created);

                    if (canInferResponseType)
                    {
                        objectTypes[metadataAttribute.StatusCode] = responseType!;
                    }
                    else if (metadataAttribute.Type != null)
                    {
                        objectTypes[metadataAttribute.StatusCode] = metadataAttribute.Type;
                    }
                }
            }

            if (objectTypes.Count == 0 && responseType != null)
            {
                objectTypes[Status200OK] = responseType;
            }

            var responseTypeMetadataProviders = MvcOptions.OutputFormatters.OfType<IApiResponseTypeMetadataProvider>();

            foreach (var objectType in objectTypes)
            {
                Type type;

                if (objectType.Value == typeof(void))
                {
                    type = objectType.Value.SubstituteIfNecessary(new TypeSubstitutionContext(serviceProvider, ModelTypeBuilder, version));

                    results.Add(new ApiResponseType()
                    {
                        StatusCode = objectType.Key,
                        Type = type,
                        ModelMetadata = MetadataProvider.GetMetadataForType(objectType.Value).SubstituteIfNecessary(type),
                    });

                    continue;
                }

                type = objectType.Value.SubstituteIfNecessary(new TypeSubstitutionContext(serviceProvider, ModelTypeBuilder, version));

                var apiResponseType = new ApiResponseType()
                {
                    StatusCode = objectType.Key,
                    Type = type,
                    ModelMetadata = MetadataProvider.GetMetadataForType(objectType.Value).SubstituteIfNecessary(type),
                };

                for (var i = 0; i < contentTypes.Count; i++)
                {
                    foreach (var responseTypeMetadataProvider in responseTypeMetadataProviders)
                    {
                        IReadOnlyList<string>? formatterSupportedContentTypes;

                        try
                        {
                            formatterSupportedContentTypes = responseTypeMetadataProvider.GetSupportedContentTypes(contentTypes[i], objectType.Value);
                        }
                        catch (InvalidOperationException)
                        {
                            // BUG: https://github.com/OData/WebApi/issues/1750
                            formatterSupportedContentTypes = null;
                        }

                        if (formatterSupportedContentTypes == null)
                        {
                            continue;
                        }

                        for (var j = 0; j < formatterSupportedContentTypes.Count; j++)
                        {
                            var responseFormat = new ApiResponseFormat()
                            {
                                Formatter = (IOutputFormatter)responseTypeMetadataProvider,
                                MediaType = formatterSupportedContentTypes[j],
                            };

                            apiResponseType.ApiResponseFormats.Add(responseFormat);
                        }
                    }
                }

                results.Add(apiResponseType);
            }

            return results;
        }

        ModelMetadata NewModelMetadata() => new ApiVersionModelMetadata(MetadataProvider, Options.DefaultApiVersionParameterDescription);

        void ProcessRouteParameters(ApiParameterContext context)
        {
            var prefix = context.RouteContext.RoutePrefix;

            if (string.IsNullOrEmpty(prefix))
            {
                return;
            }

            var routeTemplate = TemplateParser.Parse(prefix);
            var routeParameters = new Dictionary<string, ApiParameterRouteInfo>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < routeTemplate.Parameters.Count; i++)
            {
                var routeParameter = routeTemplate.Parameters[i];
                routeParameters.Add(routeParameter.Name, CreateRouteInfo(routeParameter));
            }

            for (var i = 0; i < context.Results.Count; i++)
            {
                var parameter = context.Results[i];

                if (parameter.Source == Path || parameter.Source == ModelBinding || parameter.Source == Custom)
                {
                    if (routeParameters.TryGetValue(parameter.Name, out var routeInfo))
                    {
                        parameter.RouteInfo = routeInfo;
                        routeParameters.Remove(parameter.Name);

                        if (parameter.Source == ModelBinding && !parameter.RouteInfo.IsOptional)
                        {
                            parameter.Source = Path;
                        }
                    }
                }
            }

            foreach (var routeParameter in routeParameters)
            {
                var result = new ApiParameterDescription()
                {
                    Name = routeParameter.Key,
                    RouteInfo = routeParameter.Value,
                    Source = Path,
                };

                context.Results.Add(result);

                if (!routeParameter.Value.Constraints.OfType<ApiVersionRouteConstraint>().Any())
                {
                    continue;
                }

                var metadata = NewModelMetadata();

                result.ModelMetadata = metadata;
                result.Type = metadata.ModelType;
            }
        }

        ApiParameterRouteInfo CreateRouteInfo(TemplatePart routeParameter)
        {
            var constraints = new List<IRouteConstraint>();

            if (routeParameter.InlineConstraints != null)
            {
                foreach (var constraint in routeParameter.InlineConstraints)
                {
                    constraints.Add(ConstraintResolver.ResolveConstraint(constraint.Constraint));
                }
            }

            return new ApiParameterRouteInfo()
            {
                Constraints = constraints,
                DefaultValue = routeParameter.DefaultValue,
                IsOptional = routeParameter.IsOptional || routeParameter.DefaultValue != null,
            };
        }        
    }
}