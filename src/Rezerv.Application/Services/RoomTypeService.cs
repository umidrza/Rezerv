using Rezerv.Application.DTOs.RoomType;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Services;

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;

    public RoomTypeService(
    IRoomTypeRepository roomTypeRepository,
    IHotelRepository hotelRepository,
    IHotelManagerRepository hotelManagerRepository)
    {
        _roomTypeRepository = roomTypeRepository;
        _hotelRepository = hotelRepository;
        _hotelManagerRepository = hotelManagerRepository;
    }

    public async Task<RoomTypeDto> CreateRoomTypeAsync(
        CreateRoomTypeDto dto,
        Guid userId,
        bool isAdmin)
    {
        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository
                .IsManagerAssignedAsync(userId, dto.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this hotel.");
        }
        
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Room type name is required.");

        if (dto.Capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");

        if (dto.PricePerNight <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId);

        if (hotel == null)
            throw new ArgumentException("Hotel not found.");

        var roomType = new RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = dto.HotelId,
            Name = dto.Name,
            Description = dto.Description,
            Capacity = dto.Capacity,
            PricePerNight = dto.PricePerNight
        };

        await _roomTypeRepository.AddAsync(roomType);

        return MapToDto(roomType);
    }

    public async Task<RoomTypeDto?> GetRoomTypeAsync(Guid id)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(id);

        return roomType == null ? null : MapToDto(roomType);
    }

    public async Task<List<RoomTypeDto>> GetRoomTypesByHotelAsync(Guid hotelId)
    {
        var roomTypes = await _roomTypeRepository.GetByHotelIdAsync(hotelId);

        return roomTypes.Select(MapToDto).ToList();
    }

    private static RoomTypeDto MapToDto(RoomType roomType)
    {
        return new RoomTypeDto
        {
            Id = roomType.Id,
            HotelId = roomType.HotelId,
            Name = roomType.Name,
            Description = roomType.Description,
            Capacity = roomType.Capacity,
            PricePerNight = roomType.PricePerNight
        };
    }
}