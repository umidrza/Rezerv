using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class RoomManagementRepository : IRoomManagementRepository
{
    private readonly ApplicationDbContext _context;

    public RoomManagementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Room>> GetByRoomTypeIdAsync(
        Guid roomTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .Where(r => r.RoomTypeId == roomTypeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Room room,
        CancellationToken cancellationToken = default)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Room room,
        CancellationToken cancellationToken = default)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync(cancellationToken);
    }
}