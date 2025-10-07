using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InsureYouAI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly InsureContext _context;
        public DefaultController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult SendMessage()
        {
            return PartialView();
        }

        public PartialViewResult SubscribeEmail()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult SubscribeEmail(string email)
        {
            return View();
        }
    }
}
//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