using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetByIdsAsync(List<Guid> roomIds);

    Task<bool> AreRoomsAvailableAsync(
        List<Guid> roomIds,
        DateTime checkIn,
        DateTime checkOut);
}