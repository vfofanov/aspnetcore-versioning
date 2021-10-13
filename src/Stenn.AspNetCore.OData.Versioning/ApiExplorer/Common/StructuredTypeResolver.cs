using System;
using System.Linq;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
    internal sealed class StructuredTypeResolver
    {
        private readonly IEdmModel? _model;

        internal StructuredTypeResolver(IEdmModel? model)
        {
            this._model = model;
        }

        internal IEdmStructuredType? GetStructuredType(Type type)
        {
            if (_model == null)
            {
                return default;
            }

            var structuredTypes = _model.SchemaElements.OfType<IEdmStructuredType>();
            var structuredType = structuredTypes.FirstOrDefault(t => type == t.GetClrType(_model));

            if (structuredType == null)
            {
                //TODO: Check commented code
                // var original = type.GetCustomAttribute<OriginalTypeAttribute>( inherit: false );
                //
                // if ( original != null )
                // {
                //     return GetStructuredType( original.Type );
                // }
            }

            return structuredType;
        }
    }
}