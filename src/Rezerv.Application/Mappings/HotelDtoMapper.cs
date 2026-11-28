using Rezerv.Application.DTOs.Hotel;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Mappings;

public static class HotelDtoMapper
{
    public static HotelDto Map(
        Hotel hotel,
        Dictionary<Guid, (double AverageRating, int ReviewCount)>? ratings = null)
    {
        var rating = ratings?.GetValueOrDefault(hotel.Id);

        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            StarRating = hotel.StarRating,
            PrimaryImageId = hotel.PrimaryImageId,
            PrimaryImageUrl = hotel.PrimaryImage?.ImageUrl,
            IsActive = hotel.IsActive,
            AverageRating = rating?.AverageRating ?? 0,
            ReviewCount = rating?.ReviewCount ?? 0
        };
    }
}
