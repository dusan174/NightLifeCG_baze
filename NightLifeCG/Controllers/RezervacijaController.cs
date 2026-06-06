using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NightLifeCG.Data;
using NightLifeCG.Models;

namespace NightLifeCG.Controllers
{
    public class RezervacijaController : Controller
    {
        private readonly AppDbContext _db;

        public RezervacijaController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Kreiraj(int klubId)
        {
            var klub = await _db.Klubovis
                .Include(k => k.Stolovis)
                .FirstOrDefaultAsync(k => k.KlubId == klubId);
            
            if(klub == null)
                return NotFound();

            ViewBag.Klub = klub;
            ViewBag.Stolovi = klub.Stolovis
                .Where(s => s.KlubId == klubId).ToList();
            
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Kreiraj(int stolId, int klubId, DateTime datumRezervacije, int brojGostiju)
        {
            var sto = await _db.Stolovis.FirstOrDefaultAsync(s => s.StolId == stolId);

            if (sto != null && brojGostiju > sto.BrojMjesta)
            {
                TempData["Greska"] = $"Broj gostiju ({brojGostiju}) je veći od kapaciteta stola ({sto.BrojMjesta} mjesta)!";
                return RedirectToAction("Kreiraj", new { klubId = klubId });
            }


            var postojiRezervacija = await _db.Rezervacijes
                .AnyAsync(r => r.StolId == stolId && r.DatumRezervacije.Date == datumRezervacije.Date && r.Status != 2);
            
            if(postojiRezervacija)
            {
                TempData["Greska"] = "Ovaj sto je već rezervisan za odabrani datum!";
                return RedirectToAction("Kreiraj", new { klubId = klubId });
            }

            var korisnik = await _db.Korisnicis.FirstOrDefaultAsync();

            var rezervacija = new Rezervacije
            {
                StolId = stolId,
                KorisnikId = korisnik!.KorisnikId,
                DatumRezervacije = datumRezervacije,
                BrojGostiju = brojGostiju,
                Status = 0,
                DatumKreiranja = DateTime.Now
            };

            _db.Rezervacijes.Add(rezervacija);
            await _db.SaveChangesAsync();

            TempData["Uspjeh"] = "Rezervacija je uspješno kreirana!";
            return RedirectToAction("MojeRezervacije");
        }
        public async Task<IActionResult> MojeRezervacije()
        {
            var rezervacije = await _db.Rezervacijes
                .Include(r => r.Stol)
                    .ThenInclude(s => s.Klub)
                .Include(r => r.Korisnik)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();

            return View(rezervacije);

        }

    }
}
