using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelFavoriteRepository
{
    Task<List<HotelFavorite>> GetByUserIdAsync(Guid userId);

    Task<HotelFavorite?> GetAsync(Guid userId, Guid hotelId);

    Task AddAsync(HotelFavorite favorite);

    Task DeleteAsync(HotelFavorite favorite);
}