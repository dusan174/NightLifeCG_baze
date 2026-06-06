using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NightLifeCG.Data;
using NightLifeCG.Models;

namespace NightLifeCG.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }
        
        //prikaz rezervacija
        public async Task<IActionResult> Rezervacije()
        {
            var rezervacije = await _db.Rezervacijes
                .Include(r => r.Stol)
                    .ThenInclude(s => s.Klub)
                .Include(r => r.Korisnik)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();

            return View(rezervacije);
        }
        //potvrdi rezervaciju
        public async Task<IActionResult> Potvrdi(int id)
        {
            var rezervacija = await _db.Rezervacijes.FindAsync(id);
            if(rezervacija != null)
            {
                rezervacija.Status = 1;
                await _db.SaveChangesAsync();
                TempData["Uspjeh"] = "Rezervacija je potvrđena!";
            }
            return RedirectToAction("Rezervacije");
        }

        //odbij rezervaciju
        public async Task<IActionResult> Odbij(int id)
        {
            var rezervacija = await _db.Rezervacijes.FindAsync(id);
            if (rezervacija != null)
            {
                rezervacija.Status = 2;
                await _db.SaveChangesAsync();
                TempData["Greska"] = "Rezervacija je odbijena!";
            }
            return RedirectToAction("Rezervacije");
        }

        //prikaz klubova
        public async Task<IActionResult> Klubovi()
        {
            var klubovi = await _db.Klubovis
            .Include(k => k.Grad)
            .Include(k => k.Zanr)
            .ToListAsync();

            return View(klubovi);
        }

        //dodavanje kluba(GET)

        public async Task<IActionResult> DodajKlub()
        {
            ViewBag.Gradovi = await _db.Gradovis.ToListAsync();
            ViewBag.Zanrovi = await _db.MuzickiZanrovis.ToListAsync();
            return View();
        }

        //sacuvaj klub(POST)

        [HttpPost]
        public async Task<IActionResult> DodajKlub(Klubovi klub)
        {
            klub.Aktivan = true;
            klub.ProsjecnaOcjena = 0;
            klub.BrojOcjena = 0;

            _db.Klubovis.Add(klub);
            await _db.SaveChangesAsync();

            TempData["Uspjeh"] = "Klub je uspješno dodan!";
            return RedirectToAction("Klubovi");
        }
        
        //obrisi klub
        public async Task<IActionResult> ObrisiKlub(int id)
        {
            var klub = await _db.Klubovis.FindAsync(id);
            if (klub != null)
            {
                klub.Aktivan = false;
                await _db.SaveChangesAsync();
                TempData["Uspjeh"] = "Klub je uklonjen!";
            }
            return RedirectToAction("Klubovi");
        }


    }
}
