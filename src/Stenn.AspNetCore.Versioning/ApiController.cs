using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Versioning
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : Controller
    {
    }
}