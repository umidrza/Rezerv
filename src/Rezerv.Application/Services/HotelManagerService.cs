using Rezerv.Application.DTOs.HotelManager;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Services;

public class HotelManagerService : IHotelManagerService
{
    private readonly IHotelManagerRepository _hotelManagerRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRoleService _userRoleService;

    public HotelManagerService(
        IHotelManagerRepository hotelManagerRepository,
        IHotelRepository hotelRepository,
        IUserRoleService userRoleService)
    {
        _hotelManagerRepository = hotelManagerRepository;
        _hotelRepository = hotelRepository;
        _userRoleService = userRoleService;
    }

    public async Task AssignManagerAsync(
        AssignHotelManagerDto dto,
        CancellationToken cancellationToken = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            dto.HotelId,
            cancellationToken);

        if (hotel == null)
            throw new ArgumentException("Hotel not found.");

        var alreadyAssigned = await _hotelManagerRepository
            .IsManagerAssignedAsync(
                dto.UserId,
                dto.HotelId,
                cancellationToken);

        if (alreadyAssigned)
            throw new ArgumentException("Manager is already assigned to this hotel.");

        var hotelManager = new HotelManager
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            HotelId = dto.HotelId
        };

        await _hotelManagerRepository.AddAsync(
            hotelManager,
            cancellationToken);

        await _userRoleService.AssignRoleAsync(
            dto.UserId,
            "HotelManager",
            cancellationToken);
    }
}