using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NightLifeCG.Models;

namespace NightLifeCG.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dogadjaji> Dogadjajis { get; set; }

    public virtual DbSet<Gradovi> Gradovis { get; set; }

    public virtual DbSet<Klubovi> Klubovis { get; set; }

    public virtual DbSet<Korisnici> Korisnicis { get; set; }

    public virtual DbSet<MuzickiZanrovi> MuzickiZanrovis { get; set; }

    public virtual DbSet<Recenzije> Recenzijes { get; set; }

    public virtual DbSet<Rezervacije> Rezervacijes { get; set; }

    public virtual DbSet<SlikeKluba> SlikeKlubas { get; set; }

    public virtual DbSet<Stolovi> Stolovis { get; set; }

    public virtual DbSet<Uloge> Uloges { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=NightLifeCG;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dogadjaji>(entity =>
        {
            entity.HasKey(e => e.DogadjajId).HasName("PK__Dogadjaj__060E759122718141");

            entity.ToTable("Dogadjaji");

            entity.Property(e => e.DogadjajId).HasColumnName("DogadjajID");
            entity.Property(e => e.CijenaKarte).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DatumOdrzavanja).HasColumnType("datetime");
            entity.Property(e => e.KlubId).HasColumnName("KlubID");
            entity.Property(e => e.Naziv).HasMaxLength(100);

            entity.HasOne(d => d.Klub).WithMany(p => p.Dogadjajis)
                .HasForeignKey(d => d.KlubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Dogadjaji__KlubI__6C190EBB");
        });

        modelBuilder.Entity<Gradovi>(entity =>
        {
            entity.HasKey(e => e.GradId).HasName("PK__Gradovi__B0F3C984121747A1");

            entity.ToTable("Gradovi");

            entity.Property(e => e.GradId).HasColumnName("GradID");
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        modelBuilder.Entity<Klubovi>(entity =>
        {
            entity.HasKey(e => e.KlubId).HasName("PK__Klubovi__A0FD579B23F2F4A4");

            entity.ToTable("Klubovi");

            entity.Property(e => e.KlubId).HasColumnName("KlubID");
            entity.Property(e => e.Adresa).HasMaxLength(200);
            entity.Property(e => e.Aktivan).HasDefaultValue(true);
            entity.Property(e => e.CijenaUlaza).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GradId).HasColumnName("GradID");
            entity.Property(e => e.Naziv).HasMaxLength(100);
            entity.Property(e => e.Opis).HasMaxLength(500);
            entity.Property(e => e.ZanrId).HasColumnName("ZanrID");

            entity.HasOne(d => d.Grad).WithMany(p => p.Klubovis)
                .HasForeignKey(d => d.GradId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Klubovi__GradID__59063A47");

            entity.HasOne(d => d.Zanr).WithMany(p => p.Klubovis)
                .HasForeignKey(d => d.ZanrId)
                .HasConstraintName("FK__Klubovi__ZanrID__59FA5E80");
        });

        modelBuilder.Entity<Korisnici>(entity =>
        {
            entity.HasKey(e => e.KorisnikId).HasName("PK__Korisnic__80B06D6123AB4115");

            entity.ToTable("Korisnici");

            entity.HasIndex(e => e.KorisnickoIme, "UQ__Korisnic__992E6F92E9061081").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Korisnic__A9D10534D9A31FF6").IsUnique();

            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.DatumRegistracije)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.KorisnickoIme).HasMaxLength(50);
            entity.Property(e => e.LozinkaHash).HasMaxLength(256);
            entity.Property(e => e.UlogaId).HasColumnName("UlogaID");

            entity.HasOne(d => d.Uloga).WithMany(p => p.Korisnicis)
                .HasForeignKey(d => d.UlogaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Korisnici__Uloga__5165187F");
        });

        modelBuilder.Entity<MuzickiZanrovi>(entity =>
        {
            entity.HasKey(e => e.ZanrId).HasName("PK__MuzickiZ__953868F329C40372");

            entity.ToTable("MuzickiZanrovi");

            entity.Property(e => e.ZanrId).HasColumnName("ZanrID");
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        modelBuilder.Entity<Recenzije>(entity =>
        {
            entity.HasKey(e => e.RecenzijaId).HasName("PK__Recenzij__D36C6090F50C69E4");

            entity.ToTable("Recenzije", tb =>
                {
                    tb.HasTrigger("trg_NakonBrisanjaRecenzije");
                    tb.HasTrigger("trg_NakonDodavanjaRecenzije");
                });

            entity.Property(e => e.RecenzijaId).HasColumnName("RecenzijaID");
            entity.Property(e => e.DatumOcjene)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KlubId).HasColumnName("KlubID");
            entity.Property(e => e.Komentar).HasMaxLength(500);
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");

            entity.HasOne(d => d.Klub).WithMany(p => p.Recenzijes)
                .HasForeignKey(d => d.KlubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recenzije__KlubI__693CA210");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Recenzijes)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recenzije__Koris__68487DD7");
        });

        modelBuilder.Entity<Rezervacije>(entity =>
        {
            entity.HasKey(e => e.RezervacijaId).HasName("PK__Rezervac__CABA44FD5679747A");

            entity.ToTable("Rezervacije");

            entity.Property(e => e.RezervacijaId).HasColumnName("RezervacijaID");
            entity.Property(e => e.DatumKreiranja)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DatumRezervacije).HasColumnType("datetime");
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.StolId).HasColumnName("StolID");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Rezervacijes)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rezervaci__Koris__628FA481");

            entity.HasOne(d => d.Stol).WithMany(p => p.Rezervacijes)
                .HasForeignKey(d => d.StolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rezervaci__StolI__6383C8BA");
        });

        modelBuilder.Entity<SlikeKluba>(entity =>
        {
            entity.HasKey(e => e.SlikaId).HasName("PK__SlikeKlu__FFAE2D4690C7C3F8");

            entity.ToTable("SlikeKluba");

            entity.Property(e => e.SlikaId).HasColumnName("SlikaID");
            entity.Property(e => e.KlubId).HasColumnName("KlubID");
            entity.Property(e => e.Putanja).HasMaxLength(300);

            entity.HasOne(d => d.Klub).WithMany(p => p.SlikeKlubas)
                .HasForeignKey(d => d.KlubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SlikeKlub__KlubI__6FE99F9F");
        });

        modelBuilder.Entity<Stolovi>(entity =>
        {
            entity.HasKey(e => e.StolId).HasName("PK__Stolovi__979A66434C91E70C");

            entity.ToTable("Stolovi");

            entity.Property(e => e.StolId).HasColumnName("StolID");
            entity.Property(e => e.BrojStola).HasMaxLength(20);
            entity.Property(e => e.JeVip).HasColumnName("JeVIP");
            entity.Property(e => e.KlubId).HasColumnName("KlubID");

            entity.HasOne(d => d.Klub).WithMany(p => p.Stolovis)
                .HasForeignKey(d => d.KlubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stolovi__KlubID__5DCAEF64");
        });

        modelBuilder.Entity<Uloge>(entity =>
        {
            entity.HasKey(e => e.UlogaId).HasName("PK__Uloge__DCAB23EB489FD8DD");

            entity.ToTable("Uloge");

            entity.Property(e => e.UlogaId).HasColumnName("UlogaID");
            entity.Property(e => e.Naziv).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
