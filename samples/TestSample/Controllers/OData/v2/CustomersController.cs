// Licensed under the MIT License.

using System.Linq;
using AspNetCore.OData.Versioning;
using AspNetCore.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using TestSample.Models.OData.v2;

namespace TestSample.Controllers.OData.v2
{
    [ApiVersionV2]
    public class CustomersController : ODataController<Customer>
    {
        private static Customer[] GetCustomers(ApiVersion version) => new Customer[] {
            new()
            {
                Id = 21,
                ApiVersion = version.ToString(),
                FirstName = "YXS",
                LastName = "WU",
                Email = "yxswu@abc.com"
            },
            new()
            {
                Id = 22,
                ApiVersion = version.ToString(),
                FirstName = "KIO",
                LastName = "XU",
                Email = "kioxu@efg.com"
            }
        };

        [HttpGet]
        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            return GetCustomers(this.GetApiVersion()).AsQueryable();
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var customer = GetCustomers(this.GetApiVersion()).FirstOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound($"Cannot find customer with Id={key}.");
            }

            return Ok(customer);
        }
    }
}
