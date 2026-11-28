using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IRoomTypeRepository
{
    Task<RoomType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<RoomType>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task AddAsync(
        RoomType roomType,
        CancellationToken cancellationToken = default);
}