namespace Rezerv.Domain.Entities;

public class RoomType
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal PricePerNight { get; set; }

    public Hotel Hotel { get; set; } = null!;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}