using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelManagerRepository
{
    Task<bool> IsManagerAssignedAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task AddAsync(
        HotelManager hotelManager,
        CancellationToken cancellationToken = default);
    Task<List<HotelManager>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}