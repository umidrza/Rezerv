using Rezerv.Application.DTOs.Room;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Services;

public class RoomManagementService : IRoomManagementService
{
    private readonly IRoomManagementRepository _roomRepository;
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;

    public RoomManagementService(
        IRoomManagementRepository roomRepository,
        IRoomTypeRepository roomTypeRepository,
        IHotelManagerRepository hotelManagerRepository)
    {
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
        _hotelManagerRepository = hotelManagerRepository;
    }

    public async Task<RoomDto> CreateRoomAsync(
        CreateRoomDto dto,
        Guid userId,
        bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(dto.RoomNumber))
            throw new ArgumentException("Room number is required.");

        if (dto.Floor < 0)
            throw new ArgumentException("Floor cannot be negative.");

        var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);

        if (roomType == null)
            throw new ArgumentException("Room type not found.");

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository
                .IsManagerAssignedAsync(userId, roomType.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this hotel.");
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomTypeId = dto.RoomTypeId,
            RoomNumber = dto.RoomNumber,
            Floor = dto.Floor,
            IsActive = true
        };

        await _roomRepository.AddAsync(room);

        return MapToDto(room);
    }

    public async Task<RoomDto?> GetRoomAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);

        return room == null ? null : MapToDto(room);
    }

    public async Task<List<RoomDto>> GetRoomsByRoomTypeAsync(Guid roomTypeId)
    {
        var rooms = await _roomRepository.GetByRoomTypeIdAsync(roomTypeId);

        return rooms.Select(MapToDto).ToList();
    }

    private static RoomDto MapToDto(Room room)
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