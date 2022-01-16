using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning;
using Stenn.AspNetCore.OData.Versioning.Filters;
using Stenn.AspNetCore.OData.Versioning.Operations;
using TestSample.Controllers.OData;
using TestSample.Controllers.OData.v1;
using TestSample.Models.OData;
using TestSample.Models.OData.v1;

namespace TestSample
{
    public class EdmModelFactory : EdmModelFactoryBase
    {
        /// <inheritdoc />
        public EdmModelFactory(IEdmModelMutatorFactory modelMutatorFactory,
            IEdmModelOperationExtractorFactory operationExtractorFactory)
            : base(modelMutatorFactory, operationExtractorFactory, "TestNs")
        {
        }

        
        /// <inheritdoc />
        protected override void FillModel(EdmModelBuilder builder, ApiVersion version, ApiVersion modelKey)
        {
            switch (modelKey)
            {
                case { MajorVersion: 1, MinorVersion: 0 }:
                    FillModelV1(builder);
                    break;
                case { MajorVersion: 2, MinorVersion: 0 }:
                    FillModelV2(builder);
                    break;
                case { MajorVersion: 3, MinorVersion: 0 }:
                    FillModelV3(builder);
                    break;
                default:
                    throw new NotSupportedException($"The input version '{modelKey}' is not supported!");
            }
        }

        private static void FillModelV1(EdmModelBuilder builder)
        {
            builder.Add<Book, BooksController>();
            builder.Add<Customer, CustomersController>();
        }

        private static void FillModelV2(EdmModelBuilder builder)
        {
            builder.Add<Book, BooksController>(type =>
            {
                type.AddCollectionOperation(x =>x.EBooksPost(EdmOp.ActionParams())); 
                // type.Collection
                //     .Action(nameof(BooksController.EBooksPost))
                //     .AddParameter(BooksController.ActionParams.EBooksPost.Name, c => c.Required())
                //     .AddParameter(BooksController.ActionParams.EBooksPost.Ids)
                //     .ReturnsCollectionFromEntitySet<Book, BooksController>();

                type.AddCollectionOperation(x =>x.EBooks2Post(default));
                // type.Collection
                //     .Action(nameof(BooksController.EBooks2Post))
                //     .AddParameter(BooksController.ActionParams.EBooks2Post.Ids)
                //     .ReturnsCollectionFromEntitySet<Book, BooksController>();
            });
            builder.Add<Press, PressesController>();
            builder.Add<Models.OData.v2.Customer, Controllers.OData.v2.CustomersController>();
        }
        
        private static void FillModelV3(EdmModelBuilder builder)
        {
            builder.Add<Book, BooksController>(type =>
            {
                type.AddOperation(x => x.EBooks(EdmOp.Param<string>(p => p.Required()), default));
                // type.Collection
                //     .Function(nameof(BooksController.EBooks))
                //     .AddParameter<int>("testId")
                //     .ReturnsCollectionFromEntitySet<Book, BooksController>();
            });
        }
        
        /// <inheritdoc />
        protected override void FinalizeBuilder(ODataConventionModelBuilder builder)
        {
            builder.EnableLowerCamelCase();
        }
    }
}