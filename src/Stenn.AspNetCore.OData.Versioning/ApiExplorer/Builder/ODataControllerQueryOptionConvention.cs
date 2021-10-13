using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;

namespace Microsoft.AspNet.OData.Builder
{
    internal sealed class ODataControllerQueryOptionConvention : IODataQueryOptionsConvention
    {
        private readonly ODataActionQueryOptionConventionLookup _lookup;
        private readonly ODataQueryOptionSettings _settings;

        internal ODataControllerQueryOptionConvention(
            ODataActionQueryOptionConventionLookup lookup,
            ODataQueryOptionSettings settings)
        {
            _lookup = lookup;
            _settings = settings;
        }

        public void ApplyTo(ApiDescription apiDescription)
        {
            if (!(apiDescription.ActionDescriptor is ControllerActionDescriptor action))
            {
                return;
            }

            if (!_lookup(action.MethodInfo, _settings, out var convention))
            {
                convention = ImplicitActionConvention(_settings);
            }

            convention!.ApplyTo(apiDescription);
        }

        private static IODataQueryOptionsConvention ImplicitActionConvention(ODataQueryOptionSettings settings)
        {
            var validationSettings = new ODataValidationSettings
            {
                AllowedArithmeticOperators = AllowedArithmeticOperators.None,
                AllowedFunctions = AllowedFunctions.None,
                AllowedLogicalOperators = AllowedLogicalOperators.None,
                AllowedQueryOptions = AllowedQueryOptions.None
            };

            return new ODataValidationSettingsConvention(validationSettings, settings);
        }
    }
}