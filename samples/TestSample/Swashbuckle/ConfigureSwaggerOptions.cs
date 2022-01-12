using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace TestSample.Swashbuckle
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerOptions>
    {
        public void Configure(SwaggerOptions options)
        {
            options.RouteTemplate = @"/swashbuckle/{documentName}/swagger.{json|yaml}";
        }
    }
}