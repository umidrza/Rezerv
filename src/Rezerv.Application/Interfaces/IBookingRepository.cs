using Rezerv.Domain.Entities;

namespace Rezerv.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}