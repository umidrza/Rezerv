namespace Rezerv.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }

    public Guid RoomTypeId { get; set; }
    public RoomType RoomType { get; set; } = null!;

    public string RoomNumber { get; set; } = string.Empty;

    public int Floor { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}