using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
                .ThenInclude(rt => rt.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> IsRoomAvailableAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut)
    {
        var room = await _context.Rooms
            .AnyAsync(r => r.Id == roomId && r.IsActive);

        if (!room)
            return false;

        var hasApprovedBooking = await _context.Bookings
            .AnyAsync(b =>
                b.RoomId == roomId &&
                b.Status == BookingStatus.Approved &&
                b.CheckIn < checkOut &&
                b.CheckOut > checkIn);

        return !hasApprovedBooking;
    }
}