using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelImageRepository
{
    Task<HotelImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<HotelImage>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task AddAsync(
        HotelImage hotelImage,
        CancellationToken cancellationToken = default);
    Task AddWithHotelAsync(
        HotelImage hotelImage,
        Hotel hotel,
        bool setAsPrimary,
        CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(
        List<HotelImage> hotelImages,
        CancellationToken cancellationToken = default);
    Task DeleteWithHotelAsync(
        HotelImage hotelImage,
        Hotel hotel,
        CancellationToken cancellationToken = default);
}