namespace Rezerv.Application.DTOs.Room;

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public bool IsActive { get; set; }
}