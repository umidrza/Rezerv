using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.Interfaces;

public interface IHotelService
{
    Task<HotelDto> CreateHotelAsync(CreateHotelDto dto);
    Task<HotelDto?> GetHotelAsync(Guid id);
    Task<List<HotelDto>> GetAllHotelsAsync();
    Task<PaginatedResultDto<HotelDto>> SearchHotelsAsync(SearchHotelsDto dto);
}