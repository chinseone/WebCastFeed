using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using WebCastFeed.Operations;

namespace WebCastFeed.Controllers
{
    [ApiController]
    public class StaticController : Controller
    {
        [HttpGet("MP_verify_YQh0slpbIgOFVaGv.txt")]
        public IActionResult H5Static()
        {
          return "YQh0slpbIgOFVaGv";
        }
    }
}
