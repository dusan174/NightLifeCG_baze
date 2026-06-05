using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Stolovi
{
    public int StolId { get; set; }

    public string BrojStola { get; set; } = null!;

    public int BrojMjesta { get; set; }

    public bool JeVip { get; set; }

    public int KlubId { get; set; }

    public virtual Klubovi Klub { get; set; } = null!;

    public virtual ICollection<Rezervacije> Rezervacijes { get; set; } = new List<Rezervacije>();
}
