using Rezerv.Application.DTOs.HotelReview;

namespace Rezerv.Application.Interfaces;

public interface IHotelReviewService
{
    Task<HotelReviewDto> CreateReviewAsync(
        Guid hotelId,
        Guid userId,
        CreateHotelReviewDto dto);

    Task<List<HotelReviewDto>> GetHotelReviewsAsync(Guid hotelId);
}