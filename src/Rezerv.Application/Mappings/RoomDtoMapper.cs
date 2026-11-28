using Rezerv.Application.DTOs.Room;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Mappings;

public static class RoomDtoMapper
{
    public static RoomDto Map(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            RoomTypeId = room.RoomTypeId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            IsActive = room.IsActive
        };
    }
}
