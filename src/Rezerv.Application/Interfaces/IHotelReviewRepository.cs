using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelReviewRepository
{
    Task<HotelReview?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<HotelReview>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(
        Guid hotelId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task AddAsync(
        HotelReview review,
        CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<int> GetReviewCountAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, (double AverageRating, int ReviewCount)>>
        GetRatingsByHotelIdsAsync(
            IEnumerable<Guid> hotelIds,
            CancellationToken cancellationToken = default);
}