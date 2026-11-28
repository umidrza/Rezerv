using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelFavoriteRepository
{
    Task<List<HotelFavorite>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<HotelFavorite?> GetAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        HotelFavorite favorite,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        HotelFavorite favorite,
        CancellationToken cancellationToken = default);
}