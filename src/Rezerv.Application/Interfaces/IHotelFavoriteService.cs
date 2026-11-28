using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelFavoriteService
{
    Task<List<HotelDto>> GetMyFavoritesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddFavoriteAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default);

    Task RemoveFavoriteAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default);

    Task<bool> IsFavoriteAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default);
}