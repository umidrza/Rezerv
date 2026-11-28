using System.ComponentModel.DataAnnotations;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.DTOs.RoomType;

public class CreateRoomTypeDto
{
    [NotEmptyGuid]
    public Guid HotelId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Capacity { get; set; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal PricePerNight { get; set; }
}