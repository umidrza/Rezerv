using Rezerv.Domain.Entities;
using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id);
    Task<List<Hotel>> GetAllAsync();
    Task AddAsync(Hotel hotel);
    Task UpdateAsync(Hotel hotel);
    Task<List<Hotel>> SearchAsync(SearchHotelsDto dto);
    Task<int> CountSearchResultsAsync(SearchHotelsDto dto);
}