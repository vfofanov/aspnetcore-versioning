using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

namespace Stenn.AspNetCore.OData.Versioning
{
     /// <summary>
    /// Represents a controller for generating OData service and metadata ($metadata) documents.
    /// </summary>
    public abstract class MetadataControllerBase : ControllerBase
    {
        /// <inheritdoc />
        protected MetadataControllerBase(ODataVersion oDataVersion = ODataVersion.V4)
        {
            ODataVersion = oDataVersion;
        }

        protected ODataVersion ODataVersion { get; }

        /// <summary>
        /// Generates the OData $metadata document.
        /// </summary>
        /// <returns>The <see cref="IEdmModel"/> representing $metadata.</returns>
        [HttpGet]
        public virtual IEdmModel GetMetadata()
        {
            return GetModel();
        }

        /// <summary>
        /// Generates the OData service document.
        /// </summary>
        /// <returns>The service document for the service.</returns>
        [HttpGet]
        public virtual ODataServiceDocument GetServiceDocument()
        {
            var model = GetModel();
            return model.GenerateServiceDocument();
        }
  
        [HttpOptions]
        public virtual IActionResult GetOptions()
        {
            var headers = Response.Headers;

            headers.Add("Allow", new StringValues(new[] { "GET", "OPTIONS" }));
            headers.Add(ODataConstants.ODataVersionHeader, GetODataVersionString());

            return Ok();
        }

        protected virtual IEdmModel GetModel()
        {
            var model = Request.GetModel();
            if (model == null)
            {
                throw new InvalidOperationException();
            }

            model.SetEdmxVersion(new Version(GetODataVersionString()));
            return model;
        }
        
        protected virtual string GetODataVersionString()
        {
            return ODataUtils.ODataVersionToString(ODataVersion);
        }
    }
}