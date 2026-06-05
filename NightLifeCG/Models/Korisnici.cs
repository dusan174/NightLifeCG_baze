using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Korisnici
{
    public int KorisnikId { get; set; }

    public string KorisnickoIme { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string LozinkaHash { get; set; } = null!;

    public DateTime DatumRegistracije { get; set; }

    public int UlogaId { get; set; }

    public virtual ICollection<Recenzije> Recenzijes { get; set; } = new List<Recenzije>();

    public virtual ICollection<Rezervacije> Rezervacijes { get; set; } = new List<Rezervacije>();

    public virtual Uloge Uloga { get; set; } = null!;
}
