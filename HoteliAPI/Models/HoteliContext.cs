using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HoteliAPI.Models
{
    public partial class HoteliContext : DbContext
    {
        public HoteliContext()
        {
        }

        public HoteliContext(DbContextOptions<HoteliContext> options)
            : base(options)
        {
        }
        public IConfiguration Configuration { get; }

        public virtual DbSet<Gost> Gost { get; set; }
        public virtual DbSet<Racun> Racun { get; set; }
        public virtual DbSet<Rezervacija> Rezervacija { get; set; }
        public virtual DbSet<Soba> Soba { get; set; }
        public virtual DbSet<StavkaRacuna> StavkaRacuna { get; set; }
        public virtual DbSet<Usluga> Usluga { get; set; }
        public virtual DbSet<Voditelj> Voditelj { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("HoteliDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gost>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Oib)
                    .IsRequired()
                    .HasColumnName("OIB")
                    .HasMaxLength(11);

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Racun>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Sifra)
                    .IsRequired()
                    .HasColumnName("Sifra")
                    .HasMaxLength(11);

                entity.Property(e => e.IznosPopusta).HasColumnType("decimal(3, 2)");

                entity.Property(e => e.RezervacijaId).HasColumnName("RezervacijaID");

                entity.Property(e => e.Ukupno).HasColumnType("smallmoney");

                entity.Property(e => e.Placeno).HasDefaultValue(false);

                entity.Property(e => e.UkupnoBezPopusta).HasColumnType("smallmoney");
            });

            modelBuilder.Entity<Rezervacija>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Aktivna).HasDefaultValueSql("((1))");

                entity.Property(e => e.DatumDo).HasColumnType("date");

                entity.Property(e => e.DatumOd).HasColumnType("date");

                entity.Property(e => e.GostId).HasColumnName("GostID");

                entity.Property(e => e.SobaId).HasColumnName("SobaID");

                entity.Property(e => e.VoditeljId).HasColumnName("VoditeljID");

                entity.Property(e => e.Sifra)
                    .IsRequired()
                    .HasColumnName("Sifra")
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<Soba>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.BrojSobe);

                entity.Property(e => e.CijenaPoNocenju).HasColumnType("smallmoney");
            });

            modelBuilder.Entity<StavkaRacuna>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Sifra)
                    .IsRequired()
                    .HasColumnName("Sifra")
                    .HasMaxLength(11);

                entity.Property(e => e.RacunId).HasColumnName("RacunID");

                entity.Property(e => e.UkupnaCijena).HasColumnType("smallmoney");

                entity.Property(e => e.UslugaId).HasColumnName("UslugaID");
            });

            modelBuilder.Entity<Usluga>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.JedinicnaCijena).HasColumnType("smallmoney");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Sifra)
                    .IsRequired()
                    .HasColumnName("Sifra")
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<Voditelj>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Sifra)
                    .IsRequired()
                    .HasColumnName("Sifra")
                    .HasMaxLength(11);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
