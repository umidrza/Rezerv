namespace Rezerv.Domain.Entities;

public class Hotel
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public required string Address { get; set; }

    public required string City { get; set; }

    public required string Country { get; set; }

    public int StarRating { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? PrimaryImageId { get; set; }
    public HotelImage? PrimaryImage { get; set; }

    public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    public ICollection<HotelManager> HotelManagers { get; set; } = new List<HotelManager>();
    public ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
    public ICollection<HotelReview> Reviews { get; set; } = new List<HotelReview>();

}