using System.Data;
using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsRoomBookedAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Status == BookingStatus.Approved &&
            b.CheckIn < checkOut &&
            b.CheckOut > checkIn);
    }

    public async Task<bool> ApproveAsync(Guid bookingId)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return false;

        var isBooked = await _context.Bookings.AnyAsync(b =>
            b.Id != bookingId &&
            b.RoomId == booking.RoomId &&
            b.Status == BookingStatus.Approved &&
            b.CheckIn < booking.CheckOut &&
            b.CheckOut > booking.CheckIn);

        if (isBooked)
            return false;

        booking.Status = BookingStatus.Approved;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }
}