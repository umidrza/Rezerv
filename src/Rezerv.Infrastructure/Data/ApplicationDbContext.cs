using Microsoft.EntityFrameworkCore;
using Rezerv.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Rezerv.Infrastructure.Identity;

namespace Rezerv.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingRoom> BookingRooms { get; set; }
    public DbSet<HotelManager> HotelManagers { get; set; }
    public DbSet<HotelImage> HotelImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Hotel → RoomTypes
        modelBuilder.Entity<RoomType>()
            .HasOne(rt => rt.Hotel)
            .WithMany(h => h.RoomTypes)
            .HasForeignKey(rt => rt.HotelId);

        // RoomType → Rooms
        modelBuilder.Entity<Room>()
            .HasOne(r => r.RoomType)
            .WithMany(rt => rt.Rooms)
            .HasForeignKey(r => r.RoomTypeId);

        // Hotel → Bookings
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Hotel)
            .WithMany()
            .HasForeignKey(b => b.HotelId);

        // Booking → BookingRooms
        modelBuilder.Entity<BookingRoom>()
            .HasOne(br => br.Booking)
            .WithMany(b => b.BookingRooms)
            .HasForeignKey(br => br.BookingId);

        // Room → BookingRooms
        modelBuilder.Entity<BookingRoom>()
            .HasOne(br => br.Room)
            .WithMany(r => r.BookingRooms)
            .HasForeignKey(br => br.RoomId);

        // Decimal precision
        modelBuilder.Entity<RoomType>()
            .Property(rt => rt.PricePerNight)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<BookingRoom>()
            .Property(br => br.PricePerNight)
            .HasPrecision(10, 2);

        modelBuilder.Entity<HotelManager>(entity =>
        {
            entity.HasKey(hm => hm.Id);

            entity.HasOne(hm => hm.Hotel)
                .WithMany(h => h.HotelManagers)
                .HasForeignKey(hm => hm.HotelId);

            entity.HasIndex(hm => new { hm.UserId, hm.HotelId })
                .IsUnique();
        });

        modelBuilder.Entity<HotelImage>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.HasOne(i => i.Hotel)
                .WithMany(h => h.Images)
                .HasForeignKey(i => i.HotelId);
        });

        modelBuilder.Entity<Hotel>()
            .HasOne(h => h.PrimaryImage)
            .WithMany()
            .HasForeignKey(h => h.PrimaryImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}