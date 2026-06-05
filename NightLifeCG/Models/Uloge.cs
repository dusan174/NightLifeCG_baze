using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Uloge
{
    public int UlogaId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Korisnici> Korisnicis { get; set; } = new List<Korisnici>();
}
