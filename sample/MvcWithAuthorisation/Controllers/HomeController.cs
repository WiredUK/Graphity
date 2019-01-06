using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcWithAuthorisation.Models;

namespace MvcWithAuthorisation.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var claims = User.Claims.ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
