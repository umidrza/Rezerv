using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelImageRepository
{
    Task<HotelImage?> GetByIdAsync(Guid id);
    Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId);
    Task AddAsync(HotelImage hotelImage);
    Task UpdateRangeAsync(List<HotelImage> hotelImages);
}