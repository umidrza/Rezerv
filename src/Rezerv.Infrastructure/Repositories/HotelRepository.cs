using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;
using Rezerv.Application.DTOs.Hotel;
using Rezerv.Domain.Enums;

namespace Rezerv.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel?> GetByIdAsync(Guid id)
    {
        return await _context.Hotels
            .Include(h => h.PrimaryImage)
            .Include(h => h.HotelManagers)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<List<Hotel>> GetAllAsync()
    {
        return await _context.Hotels
            .Include(h => h.PrimaryImage)
            .ToListAsync();
    }

    public async Task AddAsync(Hotel hotel)
    {
        await _context.Hotels.AddAsync(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Hotel hotel)
    {
        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Hotel>> SearchAsync(SearchHotelsDto dto)
    {
        return await BuildSearchQuery(dto)
            .Include(h => h.PrimaryImage)
            .OrderBy(h => h.Name)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();
    }

    private IQueryable<Hotel> BuildSearchQuery(SearchHotelsDto dto)
    {
        var query = _context.Hotels
            .Where(h => h.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.City))
        {
            query = query.Where(h =>
                h.City.ToLower() == dto.City.ToLower());
        }

        if (dto.StarRating.HasValue)
        {
            query = query.Where(h =>
                h.StarRating == dto.StarRating.Value);
        }

        if (dto.CheckIn.HasValue || dto.NumberOfGuests.HasValue)
        {
            var requiredCapacity = dto.NumberOfGuests ?? 1;

            query = query.Where(h =>
                h.RoomTypes
                    .SelectMany(rt => rt.Rooms)
                    .Any(r =>
                        r.IsActive &&
                        r.RoomType.Capacity >= requiredCapacity &&
                        (!dto.CheckIn.HasValue ||
                         !r.Bookings.Any(b =>
                            b.Status == BookingStatus.Approved &&
                            b.CheckIn < dto.CheckOut &&
                            b.CheckOut > dto.CheckIn))));
        }

        return query;
    }

    public async Task<int> CountSearchResultsAsync(SearchHotelsDto dto)
    {
        return await BuildSearchQuery(dto).CountAsync();
    }

    public async Task<List<Hotel>> GetByManagerIdAsync(Guid userId)
    {
        return await _context.Hotels
            .Where(h => h.HotelManagers.Any(hm => hm.UserId == userId))
            .Include(h => h.PrimaryImage)
            .OrderBy(h => h.Name)
            .ToListAsync();
    }
}