using System;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Edm
{
    internal static class EdmExtensions
    {
        internal static Type? GetClrType(this IEdmType edmType, IEdmModel edmModel)
        {
            if (edmType is not IEdmSchemaType schemaType)
            {
                return null;
            }

            var typeName = schemaType.FullName();
            var type = DeriveFromWellKnowPrimitive(typeName);

            if (type != null)
            {
                return type;
            }

            var element = schemaType;
            var annotationValue = edmModel.GetAnnotationValue<ClrTypeAnnotation>(element);

            if (annotationValue != null)
            {
                return annotationValue.ClrType;
            }

            return null;
        }

        private static Type? DeriveFromWellKnowPrimitive(string edmFullName)
        {
            switch (edmFullName)
            {
                case "Edm.String":
                case "Edm.Byte":
                case "Edm.SByte":
                case "Edm.Int16":
                case "Edm.Int32":
                case "Edm.Int64":
                case "Edm.Double":
                case "Edm.Single":
                case "Edm.Boolean":
                case "Edm.Decimal":
                case "Edm.DateTime":
                case "Edm.DateTimeOffset":
                case "Edm.Guid":
                    return Type.GetType(edmFullName.Replace("Edm", "System", StringComparison.Ordinal), true);
                case "Edm.Duration":
                    return typeof(TimeSpan);
                case "Edm.Binary":
                    return typeof(byte[]);
                case "Edm.Geography":
                case "Edm.Geometry":
                    return Type.GetType(edmFullName.Replace("Edm", "Microsoft.Spatial", StringComparison.Ordinal), true);
                case "Edm.Date":
                case "Edm.TimeOfDay":
                    return Type.GetType(edmFullName.Replace("Edm", "Microsoft.OData.Edm", StringComparison.Ordinal), true);
            }

            return null;
        }
    }
}