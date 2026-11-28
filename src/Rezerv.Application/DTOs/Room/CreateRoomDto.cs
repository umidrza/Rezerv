using System.ComponentModel.DataAnnotations;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.DTOs.Room;

public class CreateRoomDto
{
    [NotEmptyGuid]
    public Guid RoomTypeId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Floor { get; set; }
}