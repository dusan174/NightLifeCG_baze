using System;
using System.Collections.Generic;

namespace NightLifeCG.Models;

public partial class Klubovi
{
    public int KlubId { get; set; }

    public string Naziv { get; set; } = null!;

    public string? Opis { get; set; }

    public string Adresa { get; set; } = null!;

    public decimal? CijenaUlaza { get; set; }

    public double ProsjecnaOcjena { get; set; }

    public int BrojOcjena { get; set; }

    public bool Aktivan { get; set; }

    public int GradId { get; set; }

    public int? ZanrId { get; set; }

    public virtual ICollection<Dogadjaji> Dogadjajis { get; set; } = new List<Dogadjaji>();

    public virtual Gradovi Grad { get; set; } = null!;

    public virtual ICollection<Recenzije> Recenzijes { get; set; } = new List<Recenzije>();

    public virtual ICollection<SlikeKluba> SlikeKlubas { get; set; } = new List<SlikeKluba>();

    public virtual ICollection<Stolovi> Stolovis { get; set; } = new List<Stolovi>();

    public virtual MuzickiZanrovi? Zanr { get; set; }
}
