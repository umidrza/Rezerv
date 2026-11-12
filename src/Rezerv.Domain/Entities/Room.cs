namespace Rezerv.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }

    public Guid RoomTypeId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public int Floor { get; set; }

    public bool IsActive { get; set; } = true;

    public RoomType RoomType { get; set; } = null!;

    public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
}