using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stenn.AspNetCore.Versioning.Swashbuckle
{
    /// <summary>
    ///     Swashbuckle OData function param bug: workaround
    /// </summary>
    public static class ODataFunctionParamRoutingWorkaround
    {
        /// <summary>
        /// Add workaround to GenOptions
        /// </summary>
        /// <param name="options"></param>
        public static void FixGenOptions(SwaggerGenOptions options)
        {
            var defaultDocInclusionPredicate = options.SwaggerGeneratorOptions.DocInclusionPredicate;
            options.SwaggerGeneratorOptions.DocInclusionPredicate = (docName, description) =>
            {
                if (description.RelativePath != null)
                {
                    description.RelativePath = description.RelativePath.Replace("=@", EqualSignHolder + "@");
                }
                return defaultDocInclusionPredicate(docName, description);
            };
            options.DocumentFilter<Filter>();
        }

        public const string EqualSignHolder = "-----";

        private sealed class Filter : IDocumentFilter
        {
            /// <inheritdoc />
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                var replacements = swaggerDoc.Paths.Where(pair => pair.Key.Contains(EqualSignHolder)).ToList();
                foreach (var (oldKey, value) in replacements)
                {
                    swaggerDoc.Paths.Remove(oldKey);
                    var key = oldKey.Replace(EqualSignHolder, "=");
                    swaggerDoc.Paths.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Add workaround to services
        /// </summary>
        /// <param name="services"></param>
        public static void FixServices(IServiceCollection services)
        {
            services.RemoveAll(typeof(IApiDescriptionGroupCollectionProvider));
            services.AddTransient<IApiDescriptionGroupCollectionProvider, ApiDescriptionGroupCollectionProvider>();
        }
    }
}