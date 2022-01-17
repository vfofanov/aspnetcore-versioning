using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    /// Represents the metadata that describes the <see cref="ParameterConfiguration"/> associated with property/parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class ODataParamAttribute : Attribute, IParameterConfigurationInitializer
    {
        /// <summary>
        /// Optional if true otherwise required if false
        /// </summary>
        public bool IsOptional { get; set; }
        /// <summary>
        /// Gets or sets a default value for optional parameter. With set this property you will set <see cref="IsOptional"/> to true
        /// </summary>
        public string? DefaultValue { get; set; }
        
        /// <inheritdoc />
        public void Initialize(ParameterConfiguration configuration)
        {
            if (IsOptional)
            {
                configuration.Optional();
            }
            if (DefaultValue!=null)
            {
                configuration.HasDefaultValue(DefaultValue);
            }
        }
    }
}