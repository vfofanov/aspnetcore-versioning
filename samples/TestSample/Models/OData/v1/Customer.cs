// Licensed under the MIT License.

namespace TestSample.Models.OData.v1
{
    [ApiVersionV1]
    public class Customer : CustomerBase
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }
}
