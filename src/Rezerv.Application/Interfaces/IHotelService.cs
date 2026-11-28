using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.Interfaces;

public interface IHotelService
{
    Task<HotelDto> CreateHotelAsync(
        CreateHotelDto dto,
        CancellationToken cancellationToken = default);
    Task<HotelDto?> GetHotelAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<HotelDto>> GetAllHotelsAsync(
        CancellationToken cancellationToken = default);
    Task<PaginatedResultDto<HotelDto>> SearchHotelsAsync(
        SearchHotelsDto dto,
        CancellationToken cancellationToken = default);
    Task<List<HotelDto>> GetMyHotelsAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
    Task<HotelDto> UpdateHotelAsync(
        Guid hotelId,
        UpdateHotelDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
    Task SetHotelStatusAsync(
        Guid hotelId,
        bool isActive,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}