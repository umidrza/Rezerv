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

    public async Task<RoomType?> GetByIdAsync(Guid id)
    {
        return await _context.RoomTypes
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<List<RoomType>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.RoomTypes
            .Where(rt => rt.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task AddAsync(RoomType roomType)
    {
        await _context.RoomTypes.AddAsync(roomType);
        await _context.SaveChangesAsync();
    }
}