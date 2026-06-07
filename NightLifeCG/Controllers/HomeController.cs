using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NightLifeCG.Data;

namespace NightLifeCG.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.BrojKlubova = await _db.Klubovis.CountAsync(k => k.Aktivan == true);
            ViewBag.BrojGradova = await _db.Gradovis.CountAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}