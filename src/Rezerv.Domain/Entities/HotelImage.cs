namespace Rezerv.Domain.Entities;

public class HotelImage
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
}