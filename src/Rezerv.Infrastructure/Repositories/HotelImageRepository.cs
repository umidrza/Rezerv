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

    public async Task<HotelImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelImages
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<List<HotelImage>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelImages
            .Where(i => i.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        HotelImage hotelImage,
        CancellationToken cancellationToken = default)
    {
        await _context.HotelImages.AddAsync(hotelImage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddWithHotelAsync(
        HotelImage hotelImage,
        Hotel hotel,
        bool setAsPrimary,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.HotelImages.AddAsync(hotelImage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (setAsPrimary)
        {
            hotel.PrimaryImageId = hotelImage.Id;
            await _context.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(
        List<HotelImage> hotelImages,
        CancellationToken cancellationToken = default)
    {
        _context.HotelImages.UpdateRange(hotelImages);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWithHotelAsync(
        HotelImage hotelImage,
        Hotel hotel,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        if (hotel.PrimaryImageId == hotelImage.Id)
        {
            hotel.PrimaryImageId = null;
            await _context.SaveChangesAsync(cancellationToken);
        }

        _context.HotelImages.Remove(hotelImage);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}