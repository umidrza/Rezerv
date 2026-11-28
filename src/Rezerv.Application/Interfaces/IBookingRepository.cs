using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task AddIfAvailableAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<List<Booking>> GetByHotelIdAsync(Guid hotelId);
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<bool> ApproveAsync(Guid bookingId);
    Task<bool> IsRoomBookedAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
    Task<int> CountAsync(BookingStatus? status = null);

    Task<int> CountByUserIdAsync(
        Guid userId,
        BookingStatus? status = null);

    Task<int> CountByManagerIdAsync(
        Guid managerId,
        BookingStatus? status = null);

    Task<List<Booking>> GetRecentAsync(int count);

    Task<List<Booking>> GetRecentByUserIdAsync(
        Guid userId,
        int count);

    Task<List<Booking>> GetRecentByManagerIdAsync(
        Guid managerId,
        int count);
}