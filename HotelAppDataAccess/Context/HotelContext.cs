using HotelAppDataAccess.Models;
using HotelAppLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelApp.DataAccess.Context
{
    public class HotelContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public HotelContext(DbContextOptions<HotelContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<HotelModel> Hotels { get; set; }
        public DbSet<RoomTypeModel> RoomTypes { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<GuestModel> Guests { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookingModel>()
                .HasOne(b => b.Guest)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GuestId);

            modelBuilder.Entity<BookingModel>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId); 

            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.Booking)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookingId);

            modelBuilder.Entity<RoomModel>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId);
        }

    }
}
