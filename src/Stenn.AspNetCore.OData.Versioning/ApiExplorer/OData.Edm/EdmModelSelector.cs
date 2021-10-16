using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AspNetCore.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stenn.AspNetCore.OData.Versioning;

namespace Microsoft.OData.Edm
{
    /// <summary>
    ///     Represents an <see cref="IEdmModelSelector">EDM model selector</see>.
    /// </summary>
    public class EdmModelSelector : IEdmModelSelector
    {
        private readonly ApiVersion _maxVersion;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EdmModelSelector" /> class.
        /// </summary>
        /// <param name="modelProvider">The <see cref="IEdmModel">models</see> provider </param>
        /// <param name="apiVersionInfoProvider"></param>
        public EdmModelSelector(IODataModelProvider  modelProvider, 
            IApiVersionInfoProvider apiVersionInfoProvider)
        {
            var versions = new List<ApiVersion>();
            var collection = new Dictionary<ApiVersion, IEdmModel>();

            if (modelProvider == null)
            {
                throw new ArgumentNullException(nameof(modelProvider));
            }
            
            foreach (var versionInfo in apiVersionInfoProvider.Versions)
            {
                var version = versionInfo.Version;
                var model = modelProvider.GetEdmModel(version);

                collection.Add(version, model);
                versions.Add(version);
            }

            versions.Sort();
#pragma warning disable IDE0056 // Use index operator (cannot be used in web api)
            _maxVersion = versions.Count == 0 ? apiVersionInfoProvider.Default.Version : versions[^1];
#pragma warning restore IDE0056
            collection.TrimExcess();
            
            ApiVersions = versions.ToArray();
            Models = collection;
        }

        /// <inheritdoc />
        public IReadOnlyList<ApiVersion> ApiVersions { get; }

        /// <summary>
        ///     Gets the collection of EDM models.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey, TValue}">collection</see> of <see cref="IEdmModel">EDM models</see>.</value>
        protected IDictionary<ApiVersion, IEdmModel> Models { get; }

        /// <inheritdoc />
        public virtual bool Contains(ApiVersion? apiVersion)
        {
            return apiVersion != null && Models.ContainsKey(apiVersion);
        }

        /// <inheritdoc />
        public virtual IEdmModel? SelectModel(ApiVersion? apiVersion)
        {
            return apiVersion != null && Models.TryGetValue(apiVersion, out var model) ? model : default;
        }

        /// <inheritdoc />
        public virtual IEdmModel? SelectModel(IServiceProvider serviceProvider)
        {
            if (Models.Count == 0)
            {
                return default;
            }
            var version = serviceProvider.GetService<HttpRequest>()?.HttpContext.GetRequestedApiVersion();

            return version != null && Models.TryGetValue(version, out var model) ? model : Models[_maxVersion];
        }
    }
}