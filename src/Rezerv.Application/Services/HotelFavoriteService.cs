using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Rezerv.Application.Mappings;

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

    public async Task<List<HotelDto>> GetMyFavoritesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        return favorites
            .Where(f => f.Hotel.IsActive)
            .Select(f => HotelDtoMapper.Map(f.Hotel))
            .ToList();
    }

    public async Task AddFavoriteAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken);

        if (hotel == null || !hotel.IsActive)
            throw new ArgumentException("Hotel not found.");

        var existing = await _favoriteRepository.GetAsync(
            userId,
            hotelId,
            cancellationToken);

        if (existing != null)
            throw new InvalidOperationException("Hotel is already in favorites.");

        await _favoriteRepository.AddAsync(
            new Domain.Entities.HotelFavorite
            {
                UserId = userId,
                HotelId = hotelId
            },
            cancellationToken);
    }

    public async Task RemoveFavoriteAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        var favorite = await _favoriteRepository.GetAsync(
            userId,
            hotelId,
            cancellationToken);

        if (favorite == null)
            throw new ArgumentException("Favorite not found.");

        await _favoriteRepository.DeleteAsync(favorite, cancellationToken);
    }

    public async Task<bool> IsFavoriteAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        return await _favoriteRepository.GetAsync(
            userId,
            hotelId,
            cancellationToken) != null;
    }
}