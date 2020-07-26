using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelAPI.Models
{
    public partial class HotelsContext : DbContext
    {
        public HotelsContext()
        {
        }

        public HotelsContext(DbContextOptions<HotelsContext> options)
            : base(options)
        {
        }
        public IConfiguration Configuration { get; }

        public virtual DbSet<Guest> Guest { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItem { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<HotelManager> HotelManager { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("HotelsDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guest>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PIN)
                    .IsRequired()
                    .HasColumnName("PIN")
                    .HasMaxLength(11);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.PIN)
                    .IsRequired()
                    .HasColumnName("PIN")
                    .HasMaxLength(11);

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(3, 2)");

                entity.Property(e => e.ReservationId).HasColumnName("ReservationID");

                entity.Property(e => e.InTotal).HasColumnType("smallmoney");

                entity.Property(e => e.Paid).HasDefaultValue(false);

                entity.Property(e => e.TotalWithoutDiscounts).HasColumnType("smallmoney");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.DateTo).HasColumnType("date");

                entity.Property(e => e.DateFrom).HasColumnType("date");

                entity.Property(e => e.GuestId).HasColumnName("GuestId");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.HotelManagerId).HasColumnName("HotelManagerID");

                entity.Property(e => e.PIN)
                    .IsRequired()
                    .HasColumnName("PIN")
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.RoomNumber);

                entity.Property(e => e.PricePerNight).HasColumnType("smallmoney");
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.PIN)
                    .IsRequired()
                    .HasColumnName("PIN")
                    .HasMaxLength(11);

                entity.Property(e => e.InvoiceId).HasColumnName("InvoiceId");

                entity.Property(e => e.TotalPrice).HasColumnType("smallmoney");

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.PricePerItem).HasColumnType("smallmoney");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.PIN)
                    .IsRequired()
                    .HasColumnName("PIN")
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<HotelManager>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.PIN)
                    .IsRequired()
                    .HasColumnName("PIN")
                    .HasMaxLength(11);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
