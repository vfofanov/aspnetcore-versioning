using System;
using Microsoft.AspNetCore.Mvc;
using Stenn.AspNetCore.OData.Versioning;
using TestSample.Controllers.OData;
using TestSample.Controllers.OData.v1;
using TestSample.Models.OData;
using TestSample.Models.OData.v1;

namespace TestSample
{
    public class ODataModelProvider : ODataModelProviderBase
    {
        /// <inheritdoc />
        protected override void FillEdmModel(AdvODataConventionModelBuilder builder, ApiVersion key)
        {
            FillModel(builder, key);
        }

        internal static void FillModel(AdvODataConventionModelBuilder builder, ApiVersion key)
        {
            builder.Namespace = "TestNs";

            switch (key)
            {
                case { MajorVersion: 1, MinorVersion: 0 }:
                    FillModelV1(builder);
                    break;
                case { MajorVersion: 2, MinorVersion: 0 }:
                    FillModelV2(builder);
                    break;
                default:
                    throw new NotSupportedException($"The input version '{key}' is not supported!");
            }
            builder.EnableLowerCamelCase();
        }

        private static void FillModelV1(AdvODataConventionModelBuilder builder)
        {
            builder.Add<Book, BooksController>();
            builder.Add<Customer, CustomersController>();
        }

        private static void FillModelV2(AdvODataConventionModelBuilder builder)
        {
            builder.Add<Book, BooksController>(type =>
            {
                type.Collection
                    .Function(nameof(BooksController.EBooks))
                    .ReturnsCollectionFromEntitySet<Book, BooksController>();
            });
            builder.Add<Press, PressesController>();

            builder.Add<Models.OData.v2.Customer, Controllers.OData.v2.CustomersController>();
        }
    }
}