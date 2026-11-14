using Rezerv.Application.DTOs.RoomType;

namespace Rezerv.Application.Interfaces;

public interface IRoomTypeService
{
    Task<RoomTypeDto> CreateRoomTypeAsync(CreateRoomTypeDto dto, Guid userId, bool isAdmin);
    Task<RoomTypeDto?> GetRoomTypeAsync(Guid id);
    Task<List<RoomTypeDto>> GetRoomTypesByHotelAsync(Guid hotelId);
}