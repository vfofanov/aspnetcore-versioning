// Licensed under the MIT License.

namespace TestSample.Models.OData.v2
{
    [ApiVersionV2]
    public class Customer : CustomerBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
