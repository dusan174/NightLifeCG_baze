using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class MuzickiZanrovi
{
    public int ZanrId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Klubovi> Klubovis { get; set; } = new List<Klubovi>();
}
