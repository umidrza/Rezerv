using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<List<Booking>> GetByHotelIdAsync(Guid hotelId);
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<bool> ApproveAsync(Guid bookingId);
    Task<bool> IsRoomBookedAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
}