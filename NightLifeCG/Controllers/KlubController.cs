using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NightLifeCG.Data;
using NightLifeCG.Models;

namespace NightLifeCG.Controllers
{
    public class KlubController : Controller
    {
        private readonly AppDbContext _db;

        public KlubController(AppDbContext db)
        {
            _db = db;
        }  // Da automatski dobijemo konenkciju sa bazom

        public async Task<IActionResult> Index(int? gradId, int? zanrId, string? pretraga)
        {
            var klubovi = _db.Klubovis
            .Include(k => k.Grad)
            .Include(k => k.Zanr)
            .Where(k => k.Aktivan == true)
            .AsQueryable();


            if (gradId.HasValue)
                klubovi = klubovi.Where(k => k.GradId == gradId);

            if (zanrId.HasValue)
                klubovi = klubovi.Where(k => k.ZanrId == zanrId);
            
            if (!string.IsNullOrEmpty(pretraga))
                klubovi = klubovi.Where(k => k.Naziv.Contains(pretraga) || k.Adresa.Contains(pretraga));

            ViewBag.Gradovi = await _db.Gradovis.ToListAsync();
            ViewBag.Zanrovi = await _db.MuzickiZanrovis.ToListAsync();
            ViewBag.GradId = gradId;
            ViewBag.ZanrId = zanrId;
            ViewBag.Pretraga = pretraga;

            return View(await klubovi.ToListAsync());
        }
        public async Task<IActionResult> Detalji(int id)
        {
            var klub = await _db.Klubovis
            .Include(k => k.Grad)
            .Include(k => k.Zanr)
            .Include(k => k.Recenzijes)        
               .ThenInclude(r => r.Korisnik)
            .Include(k => k.Dogadjajis)
            .Include(k => k.Stolovis)
            .FirstOrDefaultAsync(k => k.KlubId == id);

            if (klub == null)
                return NotFound();
            return View(klub);

        }
    }
}
