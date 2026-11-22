using Microsoft.AspNetCore.Http;

namespace Rezerv.Application.DTOs.Hotel;

public class CreateHotelImageDto
{
    public Guid HotelId { get; set; }
    public IFormFile File { get; set; } = null!;
}