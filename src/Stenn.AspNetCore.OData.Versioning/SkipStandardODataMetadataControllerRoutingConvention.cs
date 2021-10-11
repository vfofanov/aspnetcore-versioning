using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Stenn.AspNetCore.OData.Versioning
{
    public class SkipStandardODataMetadataControllerRoutingConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            //NOTE: Skip odata metadata controller
            if (controller.ControllerType == typeof(MetadataController))
            {
                foreach (var selector in controller.Selectors)
                {
                    selector.AttributeRouteModel = null;
                }
            }
            
        }
    }
}