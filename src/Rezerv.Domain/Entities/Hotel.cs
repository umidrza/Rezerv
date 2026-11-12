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


    public Hotel(
        string name,
        string description,
        string address,
        string city,
        string country,
        int starRating)
    {
        Id = Guid.NewGuid();

        Name = name;
        Description = description;
        Address = address;
        City = city;
        Country = country;
        StarRating = starRating;

        IsActive = true;
    }
}