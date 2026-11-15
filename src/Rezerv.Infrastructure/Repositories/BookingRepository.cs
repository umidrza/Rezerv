using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;
using System.Data;
using Rezerv.Domain.Enums;

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
            .Include(b => b.BookingRooms)
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
            .Include(b => b.BookingRooms)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.BookingRooms)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ApproveAsync(Guid bookingId)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var booking = await _context.Bookings
            .Include(b => b.BookingRooms)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null || booking.Status != BookingStatus.Submitted)
            return false;

        var roomIds = booking.BookingRooms
            .Select(br => br.RoomId)
            .ToList();

        var hasConflict = await _context.BookingRooms
            .AnyAsync(br =>
                roomIds.Contains(br.RoomId) &&
                br.Booking.Status == BookingStatus.Approved &&
                br.Booking.CheckIn < booking.CheckOut &&
                br.Booking.CheckOut > booking.CheckIn);

        if (hasConflict)
            return false;

        booking.Status = BookingStatus.Approved;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }
}