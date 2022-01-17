using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning;
using Stenn.AspNetCore.OData.Versioning.Actions;
using TestSample.Models.OData;

namespace TestSample.Controllers.OData
{
    /// <summary>
    /// Books' endpoint
    /// </summary>
    public class OldBooksController : ODataController<Book>
    {
        private class EBooksPostParams : ODataActionParams
        {
            [ODataParam(DefaultValue = "attr cool!")]
            public string name { get; set; }

            public IEnumerable<int> ids { get; set; }

            /// <inheritdoc />
            public override void InitParameter(PropertyInfo propertyInfo, ParameterConfiguration configuration)
            {
                switch (propertyInfo.Name)
                {
                    case nameof(ids):
                        configuration.Optional();
                        break;
                }
            }
        }

        private class EBooks2PostParams : ODataActionParams
        {
            public IEnumerable<int> ids { get; set; }
        }

        private readonly BookStoreContext _db;

        public OldBooksController(BookStoreContext context)
        {
            _db = context;
            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (!context.Books.Any())
            {
                foreach (var b in DataSource.GetBooks())
                {
                    context.Books.Add(b);
                    context.Presses.Add(b.Press);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Returns suppliers that have deals with current user's buyer company SuppliersThatHaveDealsWithCurrentBuyer
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testId">Optional parameter</param>
        /// <returns>Returns suppliers</returns>
        [ApiVersionV3]
        [HttpGet]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public Task<IQueryable<Book>> OldEBooks([ODataParam(IsOptional = true)] string testName, int testId)
        {
            return Task.FromResult(_db.Books.Where(b => b.Press.Category == Category.EBook).AsQueryable());
        }

        /// <summary>
        /// Test post controller
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [ApiVersionV2]
        [HttpPost]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public Task<IQueryable<Book>> OldEBooksPost([EBooksPostParams]ODataActionParameters parameters)
        {
            var actionParams = parameters.Get<EBooksPostParams>();
            return Task.FromResult(_db.Books.Where(b => b.Press.Category == Category.EBook).AsQueryable());
        }
        
        [ApiVersionV2]
        [HttpPost]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public Task<IQueryable<Book>> OldEBooks2Post([EBooks2PostParams]ODataActionParameters parameters)
        {
            var actionParams = parameters.Get<EBooks2PostParams>();
            return Task.FromResult(_db.Books.Where(b => b.Press.Category == Category.EBook).AsQueryable());
        }
    }
}