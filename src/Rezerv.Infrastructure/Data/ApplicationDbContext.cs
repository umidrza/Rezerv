using Microsoft.EntityFrameworkCore;
using Rezerv.Domain.Entities;

namespace Rezerv.Infrastructure.Data;

public class ApplicationDbContext : DbContext
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
    }
}