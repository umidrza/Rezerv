namespace Rezerv.Domain.Entities;

public class HotelFavorite
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
