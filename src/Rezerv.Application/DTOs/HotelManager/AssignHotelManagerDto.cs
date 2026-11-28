using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.DTOs.HotelManager;

public class AssignHotelManagerDto
{
    [NotEmptyGuid]
    public Guid UserId { get; set; }

    [NotEmptyGuid]
    public Guid HotelId { get; set; }
}