using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Dogadjaji
{
    public int DogadjajId { get; set; }

    public string Naziv { get; set; } = null!;

    public DateTime DatumOdrzavanja { get; set; }

    public decimal? CijenaKarte { get; set; }

    public int KlubId { get; set; }

    public virtual Klubovi Klub { get; set; } = null!;
}
