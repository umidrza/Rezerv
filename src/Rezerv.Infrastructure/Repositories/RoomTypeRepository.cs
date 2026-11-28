using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly ApplicationDbContext _context;

    public RoomTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoomType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomTypes
            .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
    }

    public async Task<List<RoomType>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomTypes
            .Where(rt => rt.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        RoomType roomType,
        CancellationToken cancellationToken = default)
    {
        await _context.RoomTypes.AddAsync(roomType, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}