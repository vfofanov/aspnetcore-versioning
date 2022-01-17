using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning;
using TestSample.Controllers.OData;
using TestSample.Models.OData;

namespace TestSample
{
    public sealed class EdmModelFactory : EdmModelFactoryBase
    {
        /// <inheritdoc />
        public EdmModelFactory()
            : base("TestNs")
        {
        }

        /// <inheritdoc />
        protected override void FillModel(EdmModelBuilder builder)
        {
            builder.Add<Models.OData.v1.Customer, Controllers.OData.v1.CustomersController>();
            builder.Add<Models.OData.v2.Customer, Controllers.OData.v2.CustomersController>();

            builder.Add<Book, BooksController>(type =>
            {
                type.AddCollectionOperation(x => x.EBooksPost(default));

                type.AddCollectionOperation(x => x.EBooks2Post(default));

                type.AddCollectionOperation(x => x.EBooks(
                    EdmOp.Param<string>(p => p.Optional().HasDefaultValue("cool!!")),
                    default));
            });
            
            builder.Add<Book, OldBooksController>(type =>
            {
                type.AddCollectionOperation(x => x.OldEBooksPost(default));
                type.AddCollectionOperation(x => x.OldEBooks2Post(default));
                type.AddCollectionOperation(x => x.OldEBooks(default, default));
            });

            builder.Add<Press, PressesController>();
        }

        /// <inheritdoc />
        protected override void FinalizeBuilder(ODataConventionModelBuilder builder)
        {
            builder.EnableLowerCamelCase();
        }
    }
}