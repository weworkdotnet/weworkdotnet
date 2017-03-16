using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WeWorkDotnet.Web.Controllers
{
    public class MyJobsController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}