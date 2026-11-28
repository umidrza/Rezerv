using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;

namespace Rezerv.Application.Services;

public class HotelFavoriteService : IHotelFavoriteService
{
    private readonly IHotelFavoriteRepository _favoriteRepository;
    private readonly IHotelRepository _hotelRepository;

    public HotelFavoriteService(
        IHotelFavoriteRepository favoriteRepository,
        IHotelRepository hotelRepository)
    {
        _favoriteRepository = favoriteRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<List<HotelDto>> GetMyFavoritesAsync(Guid userId)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId);

        return favorites
            .Where(f => f.Hotel.IsActive)
            .Select(f => new HotelDto
            {
                Id = f.Hotel.Id,
                Name = f.Hotel.Name,
                Description = f.Hotel.Description,
                Address = f.Hotel.Address,
                City = f.Hotel.City,
                Country = f.Hotel.Country,
                StarRating = f.Hotel.StarRating,
                PrimaryImageId = f.Hotel.PrimaryImageId,
                PrimaryImageUrl = f.Hotel.PrimaryImage?.ImageUrl,
                IsActive = f.Hotel.IsActive
            })
            .ToList();
    }

    public async Task AddFavoriteAsync(Guid userId, Guid hotelId)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);

        if (hotel == null || !hotel.IsActive)
            throw new ArgumentException("Hotel not found.");

        var existing = await _favoriteRepository.GetAsync(userId, hotelId);

        if (existing != null)
            throw new InvalidOperationException("Hotel is already in favorites.");

        await _favoriteRepository.AddAsync(new Domain.Entities.HotelFavorite
        {
            UserId = userId,
            HotelId = hotelId
        });
    }

    public async Task RemoveFavoriteAsync(Guid userId, Guid hotelId)
    {
        var favorite = await _favoriteRepository.GetAsync(userId, hotelId);

        if (favorite == null)
            throw new ArgumentException("Favorite not found.");

        await _favoriteRepository.DeleteAsync(favorite);
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid hotelId)
    {
        return await _favoriteRepository.GetAsync(userId, hotelId) != null;
    }
}