using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Stenn.AspNetCore.Versioning.CsvRouting
{
    public class CsvRoutingMiddleware : CsvRoutingMiddlewareBase
    {
        private static readonly IReadOnlyList<string> EmptyHeaders = Array.Empty<string>();

        /// <inheritdoc />
        public CsvRoutingMiddleware(string routePattern, RequestDelegate next, char delimiter = ',') 
            : base(routePattern, next, delimiter)
        {
        }

        private static IEnumerable<EndpointRouteInfo> GetRouteInfo(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var routInfoList = new List<EndpointRouteInfo>();
            var dataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
            foreach (var endpoint in dataSource.Endpoints)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor == null)
                {
                    continue;
                }

                var routeEndpoint = endpoint as RouteEndpoint;
                var versionInfo = endpoint.Metadata.GetMetadata<ApiVersionAnnotation>()?.Info;

                var info = new EndpointRouteInfo
                (
                    controllerActionDescriptor.ControllerName,
                    endpoint.DisplayName,
                    endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? EmptyHeaders,
                    routeEndpoint?.RoutePattern.RawText ?? "N/A",
                    versionInfo?.RoutePathName ?? string.Empty
                );

                routInfoList.Add(info);
            }

            return routInfoList;
        }

        protected override string GenerateCsv(HttpContext context)
        {
            var routeInfoList = GetRouteInfo(context);

            var csvTable = new StringBuilder();
            AddRow(csvTable, "Version", "Verb", "Route", "Controller", "Method");

            foreach (var (controller, actionMethod, verbs, pattern, version) in routeInfoList.OrderBy(r => r.Pattern))
            {
                foreach (var verb in verbs)
                {
                    AddRow(csvTable, version, verb, pattern,controller, actionMethod);
                }
            }
            return csvTable.ToString();
        }

        private void AddRow(StringBuilder sbuilder, string? version, string verb, string route, string controller, string? method)
        {
            sbuilder.AppendLine(string.Join(Delimiter, GetCsvValue(version),GetCsvValue(verb), GetCsvValue(route), GetCsvValue(controller), GetCsvValue(method)));
        }

        private record EndpointRouteInfo(string Controller, string? ActionMethod, IReadOnlyList<string> HttpVerbs, string Pattern, string? VersionName);
    }
}