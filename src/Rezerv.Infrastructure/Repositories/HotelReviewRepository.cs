using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public async Task<HotelReview?> GetByIdAsync(Guid id)
    {
        return await _context.HotelReviews
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<HotelReview>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.HotelReviews
            .Where(r => r.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid hotelId, Guid userId)
    {
        return await _context.HotelReviews
            .AnyAsync(r => r.HotelId == hotelId && r.UserId == userId);
    }

    public async Task AddAsync(HotelReview review)
    {
        await _context.HotelReviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task<double> GetAverageRatingAsync(Guid hotelId)
    {
        var average = await _context.HotelReviews
            .Where(r => r.HotelId == hotelId)
            .Select(r => (double?)r.Rating)
            .AverageAsync();

        return average ?? 0;
    }

    public async Task<int> GetReviewCountAsync(Guid hotelId)
    {
        return await _context.HotelReviews
            .CountAsync(r => r.HotelId == hotelId);
    }
}