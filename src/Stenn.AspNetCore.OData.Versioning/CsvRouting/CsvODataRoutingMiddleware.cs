using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AspNetCore.Versioning.CsvRouting;

namespace Stenn.AspNetCore.OData.Versioning.CsvRouting
{
    public class CsvODataRoutingMiddleware : CsvRoutingMiddlewareBase
    {
        private static readonly IReadOnlyList<string> EmptyHeaders = Array.Empty<string>();

        /// <inheritdoc />
        public CsvODataRoutingMiddleware(string routePattern, RequestDelegate next, char delimiter = ',') 
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
                var isOData = endpoint.Metadata.GetMetadata<IODataRoutingMetadata>()!=null;

                var info = new EndpointRouteInfo
                (
                    endpoint.DisplayName,
                    endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? EmptyHeaders,
                    routeEndpoint?.RoutePattern.RawText ?? "N/A",
                    isOData
                );

                routInfoList.Add(info);
            }

            return routInfoList;
        }

        protected override string GenerateCsv(HttpContext context)
        {
            var routeInfoList = GetRouteInfo(context);

            var csvTable = new StringBuilder();
            AddRow(csvTable, "Verb", "Route", "IsOData", "Method");

            foreach (var (actionMethod, verbs, pattern, isOData) in routeInfoList.OrderBy(r => r.Pattern))
            {
                foreach (var verb in verbs)
                {
                    AddRow(csvTable, verb, pattern,isOData ? "x" : string.Empty, actionMethod);
                }
            }
            return csvTable.ToString();
        }

        private void AddRow(StringBuilder sbuilder, string verb, string route, string isodata, string? method)
        {
            sbuilder.AppendLine(string.Join(Delimiter, GetCsvValue(verb), GetCsvValue(route), GetCsvValue(isodata), GetCsvValue(method)));
        }

        private record EndpointRouteInfo(string? ActionMethod, IReadOnlyList<string> HttpVerbs, string Pattern, bool IsOData);
    }
}