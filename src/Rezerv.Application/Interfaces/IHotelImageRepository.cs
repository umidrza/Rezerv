using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelImageRepository
{
    Task<HotelImage?> GetByIdAsync(Guid id);
    Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId);
    Task AddAsync(HotelImage hotelImage);
    Task AddWithHotelAsync(
        HotelImage hotelImage,
        Hotel hotel,
        bool setAsPrimary);
    Task UpdateRangeAsync(List<HotelImage> hotelImages);
    Task DeleteWithHotelAsync(HotelImage hotelImage, Hotel hotel);
}