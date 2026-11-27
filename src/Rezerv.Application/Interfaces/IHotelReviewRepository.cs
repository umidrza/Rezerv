using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelReviewRepository
{
    Task<HotelReview?> GetByIdAsync(Guid id);
    Task<List<HotelReview>> GetByHotelIdAsync(Guid hotelId);
    Task<bool> ExistsAsync(Guid hotelId, Guid userId);
    Task AddAsync(HotelReview review);
    Task<double> GetAverageRatingAsync(Guid hotelId);
    Task<int> GetReviewCountAsync(Guid hotelId);
}