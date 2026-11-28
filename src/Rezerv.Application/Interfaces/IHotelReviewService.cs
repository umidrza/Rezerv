using Rezerv.Application.DTOs.HotelReview;

namespace Rezerv.Application.Interfaces;

public interface IHotelReviewService
{
    Task<HotelReviewDto> CreateReviewAsync(
        Guid hotelId,
        Guid userId,
        CreateHotelReviewDto dto,
        CancellationToken cancellationToken = default);

    Task<List<HotelReviewDto>> GetHotelReviewsAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);

    Task<bool> CanReviewHotelAsync(
        Guid hotelId,
        Guid userId,
        CancellationToken cancellationToken = default);
}