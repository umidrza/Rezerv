using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IRoomManagementRepository
{
    Task<Room?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<Room>> GetByRoomTypeIdAsync(
        Guid roomTypeId,
        CancellationToken cancellationToken = default);
    Task AddAsync(
        Room room,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Room room,
        CancellationToken cancellationToken = default);
}