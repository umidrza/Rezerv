using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id);
    Task<List<Hotel>> GetAllAsync();
    Task AddAsync(Hotel hotel);
}