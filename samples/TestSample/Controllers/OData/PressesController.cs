using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Stenn.AspNetCore.OData.Versioning;
using Stenn.AspNetCore.Versioning;
using TestSample.Models.OData;

namespace TestSample.Controllers.OData
{
    [ApiVersionV2]
    public class PressesController : ODataController<Press>
    {
        private readonly BookStoreContext _db;

        public PressesController(BookStoreContext context)
        {
            _db = context;
            if (context.Books.Any())
            {
                return;
            }

            foreach (var b in DataSource.GetBooks())
            {
                context.Books.Add(b);
                context.Presses.Add(b.Press);
            }

            this.GetApiVersion();
            
            context.SaveChanges();
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Press> Get()
        {
            return _db.Presses;
        }
    }
}
