using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Rezervacije
{
    public int RezervacijaId { get; set; }

    public DateTime DatumRezervacije { get; set; }

    public int BrojGostiju { get; set; }

    public int Status { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public int KorisnikId { get; set; }

    public int StolId { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual Stolovi Stol { get; set; } = null!;
}
