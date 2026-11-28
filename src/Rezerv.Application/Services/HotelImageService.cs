using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Rezerv.Application.Security;
using Rezerv.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Rezerv.Application.Services;

public class HotelImageService : IHotelImageService
{
    private readonly IHotelImageRepository _hotelImageRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<HotelImageService> _logger;

    public HotelImageService(
        IHotelImageRepository hotelImageRepository,
        IHotelRepository hotelRepository,
        IHotelManagerRepository hotelManagerRepository,
        IFileStorageService fileStorageService,
        ILogger<HotelImageService> logger)
    {
        _hotelImageRepository = hotelImageRepository;
        _hotelRepository = hotelRepository;
        _hotelManagerRepository = hotelManagerRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<HotelImageDto> AddImageAsync(
        CreateHotelImageDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var hotel = await GetAuthorizedHotelAsync(
            dto.HotelId,
            userId,
            isAdmin,
            cancellationToken);

        if (dto.File == null || dto.File.Length == 0)
            throw new ArgumentException("Image file is required.");

        await using var fileStream = dto.File.OpenReadStream();

        var imageUrl = await _fileStorageService.SaveImageAsync(
            fileStream,
            dto.File.FileName,
            dto.File.ContentType,
            cancellationToken);

        var image = new HotelImage
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            ImageUrl = imageUrl
        };

        try
        {
            await _hotelImageRepository.AddWithHotelAsync(
                image,
                hotel,
                setAsPrimary: hotel.PrimaryImageId is null,
                cancellationToken);
        }
        catch
        {
            try
            {
                await _fileStorageService.DeleteImageAsync(
                    imageUrl,
                    CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                _logger.LogWarning(
                    cleanupException,
                    "Could not remove hotel image file {ImageUrl} after metadata persistence failed.",
                    imageUrl);
            }

            throw;
        }

        return MapToDto(image);
    }

    public async Task<List<HotelImageDto>> GetHotelImagesAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        var images = await _hotelImageRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);

        return images.Select(MapToDto).ToList();
    }

    public async Task SetPrimaryImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var hotel = await GetAuthorizedHotelAsync(
            hotelId,
            userId,
            isAdmin,
            cancellationToken);

        var image = await _hotelImageRepository.GetByIdAsync(
            imageId,
            cancellationToken);

        if (image == null || image.HotelId != hotelId)
            throw new ArgumentException("Image not found.");

        hotel.PrimaryImageId = image.Id;

        await _hotelRepository.UpdateAsync(hotel, cancellationToken);
    }

    public async Task DeleteImageAsync(
        Guid hotelId,
        Guid imageId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var hotel = await GetAuthorizedHotelAsync(
            hotelId,
            userId,
            isAdmin,
            cancellationToken);

        var image = await _hotelImageRepository.GetByIdAsync(
            imageId,
            cancellationToken);

        if (image == null || image.HotelId != hotelId)
            throw new ArgumentException("Image not found.");

        await _hotelImageRepository.DeleteWithHotelAsync(
            image,
            hotel,
            cancellationToken);

        try
        {
            await _fileStorageService.DeleteImageAsync(
                image.ImageUrl,
                CancellationToken.None);
        }
        catch (Exception cleanupException)
        {
            _logger.LogWarning(
                cleanupException,
                "Could not remove hotel image file {ImageUrl} after its database record was deleted.",
                image.ImageUrl);
        }
    }

    private async Task<Hotel> GetAuthorizedHotelAsync(
        Guid hotelId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken);

        if (hotel == null)
            throw new ArgumentException("Hotel not found.");

        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            hotelId,
            isAdmin,
            cancellationToken);

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