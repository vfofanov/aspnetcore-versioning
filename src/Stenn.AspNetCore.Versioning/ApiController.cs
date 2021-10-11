using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : Controller
    {
    }
}