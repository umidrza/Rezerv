using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Rezerv.Domain.Enums;

namespace Rezerv.Infrastructure.Data.Seed;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var password = configuration["DevelopmentSeed:Password"];

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "DevelopmentSeed:Password is not configured.");
        }

        // 1. Create users
        var manager1 = await EnsureUserAsync(
            userManager,
            "manager1@rezerv.local",
            password,
            "Manager One",
            "HotelManager");

        var manager2 = await EnsureUserAsync(
            userManager,
            "manager2@rezerv.local",
            password,
            "Manager Two",
            "HotelManager");

        await EnsureUserAsync(
            userManager,
            "customer1@rezerv.local",
            password,
            "Customer One",
            "Customer");

        await EnsureUserAsync(
            userManager,
            "customer2@rezerv.local",
            password,
            "Customer Two",
            "Customer");

        // 2. Create hotels
        var hotel1 = await EnsureHotelAsync(
            context,
            "Caspian Sea Hotel",
            "Beautiful hotel with a sea view.",
            "Neftchilar Avenue 1",
            "Baku",
            "Azerbaijan",
            5,
            manager1.Id,
            "https://images.unsplash.com/photo-1566073771259-6a8506099945");

        var hotel2 = await EnsureHotelAsync(
            context,
            "Mountain View Resort",
            "Peaceful resort surrounded by mountains.",
            "Mountain Road 10",
            "Gabala",
            "Azerbaijan",
            4,
            manager1.Id,
            "https://images.unsplash.com/photo-1511895426328-dc8714191300");

        var hotel3 = await EnsureHotelAsync(
            context,
            "Old City Boutique Hotel",
            "Comfortable boutique hotel in the old city.",
            "Old City 15",
            "Baku",
            "Azerbaijan",
            3,
            manager2.Id,
            "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa");

        // 3. Create room types and physical rooms
        await EnsureRoomsAsync(context, hotel1.Id);
        await EnsureRoomsAsync(context, hotel2.Id);
        await EnsureRoomsAsync(context, hotel3.Id);

        // 4. Create Bookings 
        await EnsureBookingsAsync(context, userManager, hotel1.Id, hotel2.Id);

        await context.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var result = await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        return user;
    }

    private static async Task<Hotel> EnsureHotelAsync(
        ApplicationDbContext context,
        string name,
        string description,
        string address,
        string city,
        string country,
        int starRating,
        Guid managerId,
        string imageUrl)
    {
        var hotel = await context.Hotels
            .FirstOrDefaultAsync(h =>
                h.Name == name &&
                h.City == city &&
                h.Country == country);

        if (hotel == null)
        {
            hotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Address = address,
                City = city,
                Country = country,
                StarRating = starRating,
                IsActive = true
            };

            await context.Hotels.AddAsync(hotel);
            await context.SaveChangesAsync();
        }

        // Assign manager
        var assignmentExists = await context.HotelManagers
            .AnyAsync(hm =>
                hm.HotelId == hotel.Id &&
                hm.UserId == managerId);

        if (!assignmentExists)
        {
            await context.HotelManagers.AddAsync(new HotelManager
            {
                Id = Guid.NewGuid(),
                HotelId = hotel.Id,
                UserId = managerId
            });
        }

        // Create primary image if missing
        if (hotel.PrimaryImageId == null)
        {
            var image = await context.HotelImages
                .FirstOrDefaultAsync(i => i.HotelId == hotel.Id);

            if (image == null)
            {
                image = new HotelImage
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotel.Id,
                    ImageUrl = imageUrl
                };

                await context.HotelImages.AddAsync(image);
                await context.SaveChangesAsync();
            }

            hotel.PrimaryImageId = image.Id;
            await context.SaveChangesAsync();
        }

        return hotel;
    }

    private static async Task EnsureRoomsAsync(
        ApplicationDbContext context,
        Guid hotelId)
    {
        var roomTypes = new[]
        {
            new
            {
                Name = "Standard",
                Description = "Comfortable standard room.",
                Capacity = 2,
                Price = 80m,
                Prefix = "S"
            },
            new
            {
                Name = "Deluxe",
                Description = "Spacious deluxe room.",
                Capacity = 3,
                Price = 140m,
                Prefix = "D"
            },
            new
            {
                Name = "Suite",
                Description = "Premium suite with extra space.",
                Capacity = 4,
                Price = 220m,
                Prefix = "SU"
            }
        };

        foreach (var item in roomTypes)
        {
            var roomType = await context.RoomTypes
                .FirstOrDefaultAsync(rt =>
                    rt.HotelId == hotelId &&
                    rt.Name == item.Name);

            if (roomType == null)
            {
                roomType = new RoomType
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotelId,
                    Name = item.Name,
                    Description = item.Description,
                    Capacity = item.Capacity,
                    PricePerNight = item.Price
                };

                await context.RoomTypes.AddAsync(roomType);
                await context.SaveChangesAsync();
            }

            for (var i = 1; i <= 3; i++)
            {
                var roomNumber = $"{item.Prefix}-{i:00}";

                var roomExists = await context.Rooms.AnyAsync(r =>
                    r.RoomTypeId == roomType.Id &&
                    r.RoomNumber == roomNumber);

                if (!roomExists)
                {
                    await context.Rooms.AddAsync(new Room
                    {
                        Id = Guid.NewGuid(),
                        RoomTypeId = roomType.Id,
                        RoomNumber = roomNumber,
                        Floor = i,
                        IsActive = true
                    });
                }
            }
        }

        await context.SaveChangesAsync();
    }


    private static async Task EnsureBookingsAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        Guid hotel1Id,
        Guid hotel2Id)
    {
        var customer1 = await userManager.FindByEmailAsync("customer1@rezerv.local");
        var customer2 = await userManager.FindByEmailAsync("customer2@rezerv.local");

        if (customer1 == null || customer2 == null)
            throw new InvalidOperationException("Development customers were not seeded.");

        var standardRoom = await context.Rooms
            .Include(r => r.RoomType)
            .FirstAsync(r =>
                r.RoomType.HotelId == hotel1Id &&
                r.RoomType.Name == "Standard" &&
                r.RoomNumber == "S-01");

        var deluxeRoom = await context.Rooms
            .Include(r => r.RoomType)
            .FirstAsync(r =>
                r.RoomType.HotelId == hotel2Id &&
                r.RoomType.Name == "Deluxe" &&
                r.RoomNumber == "D-01");

        var today = DateTime.UtcNow.Date;

        if (!await context.Bookings.AnyAsync(b => b.UserId == customer1.Id))
        {
            context.Bookings.Add(new Booking
            {
                Id = Guid.NewGuid(),
                UserId = customer1.Id,
                HotelId = hotel1Id,
                RoomId = standardRoom.Id,
                CheckIn = today.AddDays(10),
                CheckOut = today.AddDays(13),
                NumberOfGuests = 2,
                PricePerNight = standardRoom.RoomType.PricePerNight,
                TotalPrice = standardRoom.RoomType.PricePerNight * 3,
                Status = BookingStatus.Submitted
            });
        }

        if (!await context.Bookings.AnyAsync(b => b.UserId == customer2.Id))
        {
            context.Bookings.Add(new Booking
            {
                Id = Guid.NewGuid(),
                UserId = customer2.Id,
                HotelId = hotel2Id,
                RoomId = deluxeRoom.Id,
                CheckIn = today.AddDays(15),
                CheckOut = today.AddDays(18),
                NumberOfGuests = 2,
                PricePerNight = deluxeRoom.RoomType.PricePerNight,
                TotalPrice = deluxeRoom.RoomType.PricePerNight * 3,
                Status = BookingStatus.Approved
            });
        }

        await context.SaveChangesAsync();
    }

}
