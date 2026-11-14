using Rezerv.Application.DTOs.Room;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Services;

public class RoomManagementService : IRoomManagementService
{
    private readonly IRoomManagementRepository _roomRepository;
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomManagementService(
        IRoomManagementRepository roomRepository,
        IRoomTypeRepository roomTypeRepository)
    {
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoomNumber))
            throw new ArgumentException("Room number is required.");

        if (dto.Floor < 0)
            throw new ArgumentException("Floor cannot be negative.");

        var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);

        if (roomType == null)
            throw new ArgumentException("Room type not found.");

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