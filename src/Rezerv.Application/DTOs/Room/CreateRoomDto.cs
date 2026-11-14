namespace Rezerv.Application.DTOs.Room;

public class CreateRoomDto
{
    public Guid RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
}