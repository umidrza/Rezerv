using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class HotelManagerRepository : IHotelManagerRepository
{
    private readonly ApplicationDbContext _context;

    public HotelManagerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsManagerAssignedAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelManagers
            .AnyAsync(
                hm => hm.UserId == userId && hm.HotelId == hotelId,
                cancellationToken);
    }

    public async Task AddAsync(
        HotelManager hotelManager,
        CancellationToken cancellationToken = default)
    {
        await _context.HotelManagers.AddAsync(hotelManager, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<HotelManager>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelManagers
            .Include(hm => hm.Hotel)
            .Where(hm => hm.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}