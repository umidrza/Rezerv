using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Services;

public class HotelImageService : IHotelImageService
{
    private readonly IHotelImageRepository _hotelImageRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;

    public HotelImageService(
        IHotelImageRepository hotelImageRepository,
        IHotelRepository hotelRepository,
        IHotelManagerRepository hotelManagerRepository)
    {
        _hotelImageRepository = hotelImageRepository;
        _hotelRepository = hotelRepository;
        _hotelManagerRepository = hotelManagerRepository;
    }

    public async Task<HotelImageDto> AddImageAsync(CreateHotelImageDto dto, Guid userId, bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(dto.ImageUrl))
            throw new ArgumentException("Image URL is required.");

        if (string.IsNullOrWhiteSpace(dto.ImageUrl) ||
            !Uri.TryCreate(dto.ImageUrl, UriKind.Absolute, out var imageUri))
        {
            throw new ArgumentException("Invalid image URL.");
        }

        if (imageUri.Scheme != Uri.UriSchemeHttp &&
            imageUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException(
                "Image URL must use HTTP or HTTPS.");
        }

        var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId);

        if (hotel == null)
            throw new ArgumentException("Hotel not found.");

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository
                .IsManagerAssignedAsync(userId, dto.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this hotel.");
        }

        var image = new HotelImage
        {
            Id = Guid.NewGuid(),
            HotelId = dto.HotelId,
            ImageUrl = dto.ImageUrl,
        };

        await _hotelImageRepository.AddAsync(image);

        return new HotelImageDto
        {
            Id = image.Id,
            HotelId = image.HotelId,
            ImageUrl = image.ImageUrl,
        };
    }

    public async Task<List<HotelImageDto>> GetHotelImagesAsync(Guid hotelId)
    {
        var images = await _hotelImageRepository.GetByHotelIdAsync(hotelId);

        return images.Select(image => new HotelImageDto
        {
            Id = image.Id,
            HotelId = image.HotelId,
            ImageUrl = image.ImageUrl
        }).ToList();
    }


    public async Task SetPrimaryImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);

        if (hotel == null)
            throw new ArgumentException("Hotel not found.");

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository
                .IsManagerAssignedAsync(userId, hotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this hotel.");
        }

        var image = await _hotelImageRepository.GetByIdAsync(imageId);

        if (image == null || image.HotelId != hotelId)
            throw new ArgumentException(
                "Image not found for this hotel.");

        hotel.PrimaryImageId = imageId;

        await _hotelRepository.UpdateAsync(hotel);
    }

}