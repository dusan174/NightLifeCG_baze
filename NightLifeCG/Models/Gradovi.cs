using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Gradovi
{
    public int GradId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Klubovi> Klubovis { get; set; } = new List<Klubovi>();
}
