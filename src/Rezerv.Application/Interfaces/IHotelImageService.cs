using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelImageService
{
    Task<HotelImageDto> AddImageAsync(
        CreateHotelImageDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<List<HotelImageDto>> GetHotelImagesAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);

    Task SetPrimaryImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task DeleteImageAsync(
       Guid hotelId,
       Guid imageId,
       Guid userId,
       bool isAdmin,
       CancellationToken cancellationToken = default);
}