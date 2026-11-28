using System.ComponentModel.DataAnnotations;

namespace Rezerv.Application.DTOs.Room;

public class UpdateRoomDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Floor { get; set; }
}