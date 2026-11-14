using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelService
{
    Task<HotelDto> CreateHotelAsync(CreateHotelDto dto);
    Task<HotelDto?> GetHotelAsync(Guid id);
    Task<List<HotelDto>> GetAllHotelsAsync();
}