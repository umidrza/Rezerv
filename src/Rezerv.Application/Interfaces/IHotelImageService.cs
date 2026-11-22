using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelImageService
{
    Task<HotelImageDto> AddImageAsync(
        CreateHotelImageDto dto,
        Guid userId,
        bool isAdmin);

    Task<List<HotelImageDto>> GetHotelImagesAsync(Guid hotelId);

    Task SetPrimaryImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin);

    Task DeleteImageAsync(
       Guid hotelId,
       Guid imageId,
       Guid userId,
       bool isAdmin);
}