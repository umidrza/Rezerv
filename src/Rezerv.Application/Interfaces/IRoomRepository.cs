using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);

    Task<bool> IsRoomAvailableAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut);
}