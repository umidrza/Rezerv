using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IRoomManagementRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<List<Room>> GetByRoomTypeIdAsync(Guid roomTypeId);
    Task AddAsync(Room room);
}