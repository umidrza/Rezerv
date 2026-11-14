using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IRoomTypeRepository
{
    Task<RoomType?> GetByIdAsync(Guid id);
    Task<List<RoomType>> GetByHotelIdAsync(Guid hotelId);
    Task AddAsync(RoomType roomType);
}