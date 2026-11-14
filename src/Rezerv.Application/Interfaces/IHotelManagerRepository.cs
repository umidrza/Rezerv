using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IHotelManagerRepository
{
    Task<bool> IsManagerAssignedAsync(Guid userId, Guid hotelId);
    Task AddAsync(HotelManager hotelManager);
    Task<List<HotelManager>> GetByUserIdAsync(Guid userId);
}