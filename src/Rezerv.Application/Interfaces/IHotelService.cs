using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.Interfaces;

public interface IHotelService
{
    Task<HotelDto> CreateHotelAsync(CreateHotelDto dto);
    Task<HotelDto?> GetHotelAsync(Guid id);
    Task<List<HotelDto>> GetAllHotelsAsync();
    Task<PaginatedResultDto<HotelDto>> SearchHotelsAsync(SearchHotelsDto dto);
    Task<List<HotelDto>> GetMyHotelsAsync(Guid userId, bool isAdmin);
    Task<HotelDto> UpdateHotelAsync(Guid hotelId, UpdateHotelDto dto, Guid userId, bool isAdmin);
    Task SetHotelStatusAsync(Guid hotelId, bool isActive, Guid userId, bool isAdmin);
}