using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
