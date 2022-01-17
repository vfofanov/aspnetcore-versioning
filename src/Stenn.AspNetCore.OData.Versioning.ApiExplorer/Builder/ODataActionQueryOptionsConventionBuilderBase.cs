using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Represents an OData controller action query options convention builder.
    /// </summary>
    public abstract class ODataActionQueryOptionsConventionBuilderBase : IODataQueryOptionsConventionBuilder
    {
        /// <summary>
        ///     Gets the validation settings used for the query options convention.
        /// </summary>
        /// <value>The <see cref="ODataValidationSettings">validation settings</see> for the convention.</value>
        protected ODataValidationSettings ValidationSettings { get; } = new()
        {
            AllowedArithmeticOperators = AllowedArithmeticOperators.None,
            AllowedFunctions = AllowedFunctions.None,
            AllowedLogicalOperators = AllowedLogicalOperators.None,
            //NOTE: Top and skip always enabled for now in OData implementation
            AllowedQueryOptions = AllowedQueryOptions.Top | AllowedQueryOptions.Skip
        };

        /// <inheritdoc />
        public virtual IODataQueryOptionsConvention Build(ODataQueryOptionSettings settings)
        {
            return new ODataValidationSettingsConvention(ValidationSettings, settings);
        }
    }
}