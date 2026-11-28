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
            .Include(h => h.Images)
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
        return await ApplySorting(BuildSearchQuery(dto), dto)
            .Include(h => h.PrimaryImage)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();
    }

    private IQueryable<Hotel> BuildSearchQuery(SearchHotelsDto dto)
    {
        var query = _context.Hotels
            .Where(h => h.IsActive);

        if (!string.IsNullOrWhiteSpace(dto.City))
        {
            query = query.Where(h =>
                h.City.ToLower() == dto.City.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(dto.Country))
        {
            query = query.Where(h =>
                h.Country.ToLower() == dto.Country.ToLower());
        }

        if (dto.StarRating.HasValue)
        {
            query = query.Where(h =>
                h.StarRating == dto.StarRating.Value);
        }

        if (dto.MinimumAverageRating.HasValue)
        {
            query = query.Where(h =>
                _context.HotelReviews
                    .Where(r => r.HotelId == h.Id)
                    .Average(r => (double?)r.Rating) >= dto.MinimumAverageRating);
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
                            (b.Status == BookingStatus.Submitted ||
                             b.Status == BookingStatus.Approved) &&
                            b.CheckIn < dto.CheckOut &&
                            b.CheckOut > dto.CheckIn))));
        }

        return query;
    }

    private IOrderedQueryable<Hotel> ApplySorting(
        IQueryable<Hotel> query,
        SearchHotelsDto dto)
    {
        return dto.SortBy?.ToLower() switch
        {
            "name" => dto.SortDescending
                ? query.OrderByDescending(h => h.Name)
                : query.OrderBy(h => h.Name),

            "starrating" => dto.SortDescending
                ? query.OrderByDescending(h => h.StarRating)
                : query.OrderBy(h => h.StarRating),

            "averagerating" => dto.SortDescending
                ? query.OrderByDescending(h =>
                    _context.HotelReviews
                        .Where(r => r.HotelId == h.Id)
                        .Average(r => (double?)r.Rating) ?? 0)
                : query.OrderBy(h =>
                    _context.HotelReviews
                        .Where(r => r.HotelId == h.Id)
                        .Average(r => (double?)r.Rating) ?? 0),

            _ => query.OrderBy(h => h.Name)
        };
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

    public async Task<int> CountAsync()
    {
        return await _context.Hotels.CountAsync();
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Hotels
            .CountAsync(h => h.IsActive);
    }

    public async Task<int> CountByManagerIdAsync(Guid managerId)
    {
        return await _context.Hotels
            .CountAsync(h => h.HotelManagers.Any(
                hm => hm.UserId == managerId));
    }

    public async Task<int> CountActiveByManagerIdAsync(Guid managerId)
    {
        return await _context.Hotels
            .CountAsync(h =>
                h.IsActive &&
                h.HotelManagers.Any(hm => hm.UserId == managerId));
    }
}