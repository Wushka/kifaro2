using kifaro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace kifaro.Controllers
{
    public class HomeController : Controller
    {
        private readonly kifaroContext _context;

        public HomeController(kifaroContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var objList = _context.Set<User>().ToArray();
            return View(objList);
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