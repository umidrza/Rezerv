using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class HotelImageRepository : IHotelImageRepository
{
    private readonly ApplicationDbContext _context;

    public HotelImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HotelImage?> GetByIdAsync(Guid id)
    {
        return await _context.HotelImages
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.HotelImages
            .Where(i => i.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task AddAsync(HotelImage hotelImage)
    {
        await _context.HotelImages.AddAsync(hotelImage);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<HotelImage> hotelImages)
    {
        _context.HotelImages.UpdateRange(hotelImages);
        await _context.SaveChangesAsync();
    }
}