using Rezerv.Application.DTOs;

namespace Rezerv.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, Guid userId);
}