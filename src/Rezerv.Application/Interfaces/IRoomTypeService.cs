using Rezerv.Application.DTOs.RoomType;

namespace Rezerv.Application.Interfaces;

public interface IRoomTypeService
{
    Task<RoomTypeDto> CreateRoomTypeAsync(
        CreateRoomTypeDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
    Task<RoomTypeDto?> GetRoomTypeAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<RoomTypeDto>> GetRoomTypesByHotelAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
}