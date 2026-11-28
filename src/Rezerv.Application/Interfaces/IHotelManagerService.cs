using Rezerv.Application.DTOs.HotelManager;

namespace Rezerv.Application.Interfaces;

public interface IHotelManagerService
{
    Task AssignManagerAsync(
        AssignHotelManagerDto dto,
        CancellationToken cancellationToken = default);
}