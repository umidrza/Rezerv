using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelFavoriteService
{
    Task<List<HotelDto>> GetMyFavoritesAsync(Guid userId);

    Task AddFavoriteAsync(Guid userId, Guid hotelId);

    Task RemoveFavoriteAsync(Guid userId, Guid hotelId);

    Task<bool> IsFavoriteAsync(Guid userId, Guid hotelId);
}