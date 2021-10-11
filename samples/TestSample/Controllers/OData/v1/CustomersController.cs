// Licensed under the MIT License.

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Stenn.AspNetCore.OData.Versioning;
using Stenn.AspNetCore.Versioning;
using TestSample.Models.OData.v1;

namespace TestSample.Controllers.OData.v1
{
    [ApiVersionV1]
    public class CustomersController : ODataController<Customer>
    {
        private static Customer[] GetCustomers(ApiVersion version) => new Customer[] {
            new()
            {
                Id = 11,
                ApiVersion = version.ToString(),
                Name = "Sam",
                PhoneNumber = "111-222-3333"
            },
            new()
            {
                Id = 12,
                ApiVersion = version.ToString(),
                Name = "Peter",
                PhoneNumber = "456-ABC-8888"
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
