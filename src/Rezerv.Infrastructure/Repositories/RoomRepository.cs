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

    public async Task<List<Room>> GetByIdsAsync(List<Guid> roomIds)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .Where(r => roomIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<bool> AreRoomsAvailableAsync(
        List<Guid> roomIds,
        DateTime checkIn,
        DateTime checkOut)
    {
        var hasConflict = await _context.BookingRooms
            .AnyAsync(br =>
                roomIds.Contains(br.RoomId) &&
                br.Booking.Status == BookingStatus.Approved &&
                br.Booking.CheckIn < checkOut &&
                br.Booking.CheckOut > checkIn);

        return !hasConflict;
    }
}