using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Recenzije
{
    public int RecenzijaId { get; set; }

    public int Ocjena { get; set; }

    public string? Komentar { get; set; }

    public DateTime DatumOcjene { get; set; }

    public int KorisnikId { get; set; }

    public int KlubId { get; set; }

    public virtual Klubovi Klub { get; set; } = null!;

    public virtual Korisnici Korisnik { get; set; } = null!;
}
