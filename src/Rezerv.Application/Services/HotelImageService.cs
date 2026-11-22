using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;

namespace Rezerv.Application.Services;

public class HotelImageService : IHotelImageService
{
    private readonly IHotelImageRepository _hotelImageRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;
    private readonly IFileStorageService _fileStorageService;

    public HotelImageService(
        IHotelImageRepository hotelImageRepository,
        IHotelRepository hotelRepository,
        IHotelManagerRepository hotelManagerRepository,
        IFileStorageService fileStorageService)
    {
        _hotelImageRepository = hotelImageRepository;
        _hotelRepository = hotelRepository;
        _hotelManagerRepository = hotelManagerRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<HotelImageDto> AddImageAsync(
        CreateHotelImageDto dto,
        Guid userId,
        bool isAdmin)
    {
        var hotel = await GetAuthorizedHotelAsync(dto.HotelId, userId, isAdmin);

        if (dto.File == null || dto.File.Length == 0)
            throw new ArgumentException("Image file is required.");

        await using var fileStream = dto.File.OpenReadStream();

        var imageUrl = await _fileStorageService.SaveImageAsync(
            fileStream,
            dto.File.FileName,
            dto.File.ContentType);

        var image = new HotelImage
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            ImageUrl = imageUrl
        };

        await _hotelImageRepository.AddAsync(image);

        // Automatically select the first uploaded image as primary.
        if (hotel.PrimaryImageId == null)
        {
            hotel.PrimaryImageId = image.Id;
            await _hotelRepository.UpdateAsync(hotel);
        }

        return MapToDto(image);
    }

    public async Task<List<HotelImageDto>> GetHotelImagesAsync(Guid hotelId)
    {
        var images = await _hotelImageRepository.GetByHotelIdAsync(hotelId);

        return images.Select(MapToDto).ToList();
    }

    public async Task SetPrimaryImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin)
    {
        var hotel = await GetAuthorizedHotelAsync(hotelId, userId, isAdmin);

        var image = await _hotelImageRepository.GetByIdAsync(imageId);

        if (image == null || image.HotelId != hotelId)
            throw new ArgumentException("Image not found.");

        hotel.PrimaryImageId = image.Id;

        await _hotelRepository.UpdateAsync(hotel);
    }

    public async Task DeleteImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin)
    {
        var hotel = await GetAuthorizedHotelAsync(hotelId, userId, isAdmin);

        var image = await _hotelImageRepository.GetByIdAsync(imageId);

        if (image == null || image.HotelId != hotelId)
            throw new ArgumentException("Image not found.");

        if (hotel.PrimaryImageId == imageId)
        {
            hotel.PrimaryImageId = null;
            await _hotelRepository.UpdateAsync(hotel);
        }

        await _hotelImageRepository.DeleteAsync(image);
        await _fileStorageService.DeleteImageAsync(image.ImageUrl);
    }

    private async Task<Hotel> GetAuthorizedHotelAsync(
        Guid hotelId,
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

        return hotel;
    }

    private static HotelImageDto MapToDto(HotelImage image)
    {
        return new HotelImageDto
        {
            Id = image.Id,
            HotelId = image.HotelId,
            ImageUrl = image.ImageUrl
        };
    }
}