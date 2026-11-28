using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;
using Rezerv.Infrastructure.Identity;

namespace Rezerv.Infrastructure.Repositories;

public class HotelReviewRepository : IHotelReviewRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HotelReviewRepository(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<HotelReview?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelReviews
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<HotelReview>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelReviews
            .Where(r => r.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid hotelId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelReviews
            .AnyAsync(
                r => r.HotelId == hotelId && r.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        HotelReview review,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.HotelReviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.GetBaseException() is PostgresException postgresException &&
                  postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
                  postgresException.ConstraintName == "IX_HotelReviews_HotelId_UserId")
        {
            throw new InvalidOperationException(
                "You have already reviewed this hotel.",
                ex);
        }
    }

    public async Task<double> GetAverageRatingAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        var average = await _context.HotelReviews
            .Where(r => r.HotelId == hotelId)
            .Select(r => (double?)r.Rating)
            .AverageAsync(cancellationToken);

        return average ?? 0;
    }

    public async Task<int> GetReviewCountAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HotelReviews
            .CountAsync(r => r.HotelId == hotelId, cancellationToken);
    }

    public async Task<Dictionary<Guid, (double AverageRating, int ReviewCount)>>
    GetRatingsByHotelIdsAsync(
        IEnumerable<Guid> hotelIds,
        CancellationToken cancellationToken = default)
    {
        var ids = hotelIds.Distinct().ToList();

        return await _context.HotelReviews
            .Where(r => ids.Contains(r.HotelId))
            .GroupBy(r => r.HotelId)
            .Select(g => new
            {
                HotelId = g.Key,
                AverageRating = g.Average(r => (double)r.Rating),
                ReviewCount = g.Count()
            })
            .ToDictionaryAsync(
                x => x.HotelId,
                x => (x.AverageRating, x.ReviewCount),
                cancellationToken);
    }
}