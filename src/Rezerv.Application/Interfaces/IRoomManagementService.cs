using Rezerv.Application.DTOs.Room;

namespace Rezerv.Application.Interfaces;

public interface IRoomManagementService
{
    Task<RoomDto> CreateRoomAsync(
        CreateRoomDto dto,
        Guid userId,
        bool isAdmin);
    Task<RoomDto?> GetRoomAsync(Guid id);
    Task<List<RoomDto>> GetRoomsByRoomTypeAsync(Guid roomTypeId);
}