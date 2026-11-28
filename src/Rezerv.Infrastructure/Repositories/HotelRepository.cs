using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Application.DTOs.Dashboard;
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

    public async Task<Hotel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .Include(h => h.PrimaryImage)
            .Include(h => h.Images)
            .Include(h => h.HotelManagers)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<List<Hotel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .Include(h => h.PrimaryImage)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Hotel hotel,
        CancellationToken cancellationToken = default)
    {
        await _context.Hotels.AddAsync(hotel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Hotel hotel,
        CancellationToken cancellationToken = default)
    {
        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Hotel>> SearchAsync(
        SearchHotelsDto dto,
        CancellationToken cancellationToken = default)
    {
        return await ApplySorting(BuildSearchQuery(dto), dto)
            .Include(h => h.PrimaryImage)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync(cancellationToken);
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

    public async Task<int> CountSearchResultsAsync(
        SearchHotelsDto dto,
        CancellationToken cancellationToken = default)
    {
        return await BuildSearchQuery(dto).CountAsync(cancellationToken);
    }

    public async Task<List<Hotel>> GetByManagerIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .Where(h => h.HotelManagers.Any(hm => hm.UserId == userId))
            .Include(h => h.PrimaryImage)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels.CountAsync(cancellationToken);
    }

    public async Task<int> CountActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .CountAsync(h => h.IsActive, cancellationToken);
    }

    public async Task<int> CountByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .CountAsync(h => h.HotelManagers.Any(
                hm => hm.UserId == managerId),
                cancellationToken);
    }

    public async Task<int> CountActiveByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .CountAsync(h =>
                h.IsActive &&
                h.HotelManagers.Any(hm => hm.UserId == managerId),
                cancellationToken);
    }

    public Task<HotelDashboardCounts> GetDashboardCountsAsync(
        CancellationToken cancellationToken = default)
    {
        return GetDashboardCountsAsync(_context.Hotels, cancellationToken);
    }

    public Task<HotelDashboardCounts> GetDashboardCountsByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default)
    {
        var hotels = _context.Hotels.Where(h =>
            h.HotelManagers.Any(hm => hm.UserId == managerId));
        return GetDashboardCountsAsync(hotels, cancellationToken);
    }

    private static async Task<HotelDashboardCounts> GetDashboardCountsAsync(
        IQueryable<Hotel> hotels,
        CancellationToken cancellationToken)
    {
        return await hotels
            .GroupBy(_ => 1)
            .Select(group => new HotelDashboardCounts(
                group.Count(),
                group.Count(hotel => hotel.IsActive)))
            .SingleOrDefaultAsync(cancellationToken)
            ?? new HotelDashboardCounts(0, 0);
    }
}