using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class HotelFavoriteRepository : IHotelFavoriteRepository
{
    private readonly ApplicationDbContext _context;

    public HotelFavoriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HotelFavorite>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelFavorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Hotel)
                .ThenInclude(h => h.PrimaryImage)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<HotelFavorite?> GetAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelFavorites
            .FirstOrDefaultAsync(f =>
                f.UserId == userId &&
                f.HotelId == hotelId,
                cancellationToken);
    }

    public async Task AddAsync(
        HotelFavorite favorite,
        CancellationToken cancellationToken = default)
    {
        await _context.HotelFavorites.AddAsync(favorite, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        HotelFavorite favorite,
        CancellationToken cancellationToken = default)
    {
        _context.HotelFavorites.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);
    }
}