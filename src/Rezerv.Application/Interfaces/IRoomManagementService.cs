using Rezerv.Application.DTOs.Room;

namespace Rezerv.Application.Interfaces;

public interface IRoomManagementService
{
    Task<RoomDto> CreateRoomAsync(
        CreateRoomDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
    Task<RoomDto?> GetRoomAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<RoomDto>> GetRoomsByRoomTypeAsync(
        Guid roomTypeId,
        CancellationToken cancellationToken = default);
    Task<RoomDto> UpdateRoomAsync(
        Guid id,
        UpdateRoomDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<RoomDto> ToggleRoomStatusAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}