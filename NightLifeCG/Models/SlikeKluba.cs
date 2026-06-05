using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class SlikeKluba
{
    public int SlikaId { get; set; }

    public string Putanja { get; set; } = null!;

    public bool Glavna { get; set; }

    public int KlubId { get; set; }

    public virtual Klubovi Klub { get; set; } = null!;
}
