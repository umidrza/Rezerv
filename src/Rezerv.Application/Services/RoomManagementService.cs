using Rezerv.Application.DTOs.Room;
using Rezerv.Application.Interfaces;
using Rezerv.Application.Mappings;
using Rezerv.Application.Security;
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
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.RoomNumber))
            throw new ArgumentException("Room number is required.");

        if (dto.Floor < 0)
            throw new ArgumentException("Floor cannot be negative.");

        var roomType = await _roomTypeRepository.GetByIdAsync(
            dto.RoomTypeId,
            cancellationToken);

        if (roomType == null)
            throw new ArgumentException("Room type not found.");

        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            roomType.HotelId,
            isAdmin,
            cancellationToken);

        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomTypeId = dto.RoomTypeId,
            RoomNumber = dto.RoomNumber,
            Floor = dto.Floor,
            IsActive = true
        };

        await _roomRepository.AddAsync(room, cancellationToken);

        return RoomDtoMapper.Map(room);
    }

    public async Task<RoomDto?> GetRoomAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        return room == null ? null : RoomDtoMapper.Map(room);
    }

    public async Task<List<RoomDto>> GetRoomsByRoomTypeAsync(
        Guid roomTypeId,
        CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.GetByRoomTypeIdAsync(
            roomTypeId,
            cancellationToken);

        return rooms.Select(RoomDtoMapper.Map).ToList();
    }

    public async Task<RoomDto> UpdateRoomAsync(
        Guid id,
        UpdateRoomDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room == null)
            throw new ArgumentException("Room not found.");

        if (string.IsNullOrWhiteSpace(dto.RoomNumber))
            throw new ArgumentException("Room number is required.");

        if (dto.Floor < 0)
            throw new ArgumentException("Floor cannot be negative.");

        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            room.RoomType.HotelId,
            isAdmin,
            cancellationToken);

        room.RoomNumber = dto.RoomNumber;
        room.Floor = dto.Floor;

        await _roomRepository.UpdateAsync(room, cancellationToken);

        return RoomDtoMapper.Map(room);
    }

    public async Task<RoomDto> ToggleRoomStatusAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room == null)
            throw new ArgumentException("Room not found.");

        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            room.RoomType.HotelId,
            isAdmin,
            cancellationToken);

        room.IsActive = !room.IsActive;

        await _roomRepository.UpdateAsync(room, cancellationToken);

        return RoomDtoMapper.Map(room);
    }
}