using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Application.DTOs.Common;
using Rezerv.Application.Mappings;
using Rezerv.Application.Security;


namespace Rezerv.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelImageRepository _hotelImageRepository;
    private readonly IHotelReviewRepository _reviewRepository;

    public HotelService(
        IHotelRepository hotelRepository,
        IHotelImageRepository hotelImageRepository,
        IHotelReviewRepository reviewRepository)
    {
        _hotelRepository = hotelRepository;
        _hotelImageRepository = hotelImageRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<HotelDto> CreateHotelAsync(
        CreateHotelDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Hotel name is required.");

        if (dto.StarRating < 1 || dto.StarRating > 5)
            throw new ArgumentException("Star rating must be between 1 and 5.");

        if (!string.IsNullOrWhiteSpace(dto.PrimaryImageUrl))
        {
            if (!Uri.TryCreate(dto.PrimaryImageUrl, UriKind.Absolute, out var imageUri))
            {
                throw new ArgumentException("Invalid primary image URL.");
            }

            if (imageUri.Scheme != Uri.UriSchemeHttp &&
                imageUri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Image URL must use HTTP or HTTPS.");
            }
        }

        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            StarRating = dto.StarRating,
            IsActive = true
        };

        await _hotelRepository.AddAsync(hotel, cancellationToken);

        if (!string.IsNullOrWhiteSpace(dto.PrimaryImageUrl))
        {
            var image = new HotelImage
            {
                Id = Guid.NewGuid(),
                HotelId = hotel.Id,
                ImageUrl = dto.PrimaryImageUrl
            };

            await _hotelImageRepository.AddAsync(image, cancellationToken);

            hotel.PrimaryImageId = image.Id;
            hotel.PrimaryImage = image;

            await _hotelRepository.UpdateAsync(hotel, cancellationToken);
        }

        return HotelDtoMapper.Map(hotel);
    }

    public async Task<HotelDto?> GetHotelAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id, cancellationToken);

        if (hotel == null)
            return null;

        var ratings = await _reviewRepository.GetRatingsByHotelIdsAsync(
            new[] { hotel.Id },
            cancellationToken);

        return HotelDtoMapper.Map(hotel, ratings);
    }

    public async Task<List<HotelDto>> GetAllHotelsAsync(
        CancellationToken cancellationToken = default)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);

        var ratings = await _reviewRepository.GetRatingsByHotelIdsAsync(
            hotels.Select(h => h.Id),
            cancellationToken);

        return hotels.Select(h => HotelDtoMapper.Map(h, ratings)).ToList();
    }

    public async Task<PaginatedResultDto<HotelDto>> SearchHotelsAsync(
        SearchHotelsDto dto,
        CancellationToken cancellationToken = default)
    {
        // Validate search filters
        if (dto.StarRating.HasValue &&
            (dto.StarRating < 1 || dto.StarRating > 5))
        {
            throw new ArgumentException("Star rating must be between 1 and 5.");
        }

        if (dto.MinimumAverageRating.HasValue &&
            (dto.MinimumAverageRating < 1 || dto.MinimumAverageRating > 5))
        {
            throw new ArgumentException("Minimum average rating must be between 1 and 5.");
        }

        if (!string.IsNullOrWhiteSpace(dto.SortBy) &&
            !new[] { "Name", "StarRating", "AverageRating" }
                .Contains(dto.SortBy, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid sort option.");
        }

        if (dto.CheckIn.HasValue != dto.CheckOut.HasValue)
        {
            throw new ArgumentException("Both check-in and check-out dates are required.");
        }

        if (dto.CheckIn.HasValue)
        {
            if (dto.CheckOut <= dto.CheckIn)
            {
                throw new ArgumentException("Check-out must be after check-in.");
            }

            if (dto.CheckIn.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Check-in date cannot be in the past.");
            }
        }

        if (dto.NumberOfGuests.HasValue && dto.NumberOfGuests <= 0)
        {
            throw new ArgumentException("Number of guests must be greater than zero.");
        }

        if (dto.Page < 1)
        {
            throw new ArgumentException("Page must be at least 1.");
        }

        if (dto.PageSize < 1 || dto.PageSize > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100.");
        }

        // Ensure dates are UTC
        if (dto.CheckIn.HasValue)
        {
            dto.CheckIn = DateTime.SpecifyKind(dto.CheckIn.Value, DateTimeKind.Utc);
            dto.CheckOut = DateTime.SpecifyKind(dto.CheckOut!.Value, DateTimeKind.Utc);
        }

        var hotels = await _hotelRepository.SearchAsync(dto, cancellationToken);
        var totalCount = await _hotelRepository.CountSearchResultsAsync(
            dto,
            cancellationToken);

        var ratings = await _reviewRepository.GetRatingsByHotelIdsAsync(
            hotels.Select(h => h.Id),
            cancellationToken);

        var items = hotels
            .Select(hotel => HotelDtoMapper.Map(hotel, ratings))
            .ToList();

        return new PaginatedResultDto<HotelDto>
        {
            Items = items,
            Page = dto.Page,
            PageSize = dto.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize)
        };
    }

    public async Task<List<HotelDto>> GetMyHotelsAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var hotels = isAdmin
            ? await _hotelRepository.GetAllAsync(cancellationToken)
            : await _hotelRepository.GetByManagerIdAsync(
                userId,
                cancellationToken);

        var ratings = await _reviewRepository.GetRatingsByHotelIdsAsync(
            hotels.Select(h => h.Id),
            cancellationToken);

        return hotels.Select(h => HotelDtoMapper.Map(h, ratings)).ToList();
    }

    public async Task<HotelDto> UpdateHotelAsync(
        Guid hotelId,
        UpdateHotelDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken);

        if (hotel == null)
            throw new KeyNotFoundException("Hotel not found.");

        HotelManagerAuthorization.EnsureCanManageHotel(
            isAdmin,
            hotel.HotelManagers.Any(hm => hm.UserId == userId));

        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Hotel name is required.");

        if (dto.StarRating < 1 || dto.StarRating > 5)
            throw new ArgumentException("Star rating must be between 1 and 5.");

        if (!string.IsNullOrWhiteSpace(dto.PrimaryImageUrl))
        {
            if (!Uri.TryCreate(dto.PrimaryImageUrl, UriKind.Absolute, out var imageUri) ||
                (imageUri.Scheme != Uri.UriSchemeHttp &&
                 imageUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid primary image URL.");
            }
        }

        hotel.Name = dto.Name;
        hotel.Description = dto.Description;
        hotel.Address = dto.Address;
        hotel.City = dto.City;
        hotel.Country = dto.Country;
        hotel.StarRating = dto.StarRating;

        // Update or create primary image
        if (!string.IsNullOrWhiteSpace(dto.PrimaryImageUrl))
        {
            if (hotel.PrimaryImage != null)
            {
                hotel.PrimaryImage.ImageUrl = dto.PrimaryImageUrl;

                await _hotelImageRepository.UpdateRangeAsync(
                    new List<HotelImage> { hotel.PrimaryImage },
                    cancellationToken);
            }
            else
            {
                var image = new HotelImage
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotel.Id,
                    ImageUrl = dto.PrimaryImageUrl
                };

                await _hotelImageRepository.AddAsync(image, cancellationToken);

                hotel.PrimaryImageId = image.Id;
                hotel.PrimaryImage = image;
            }
        }

        await _hotelRepository.UpdateAsync(hotel, cancellationToken);

        return HotelDtoMapper.Map(hotel);
    }


    public async Task SetHotelStatusAsync(
        Guid hotelId,
        bool isActive,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken);

        if (hotel == null)
            throw new KeyNotFoundException("Hotel not found.");

        HotelManagerAuthorization.EnsureCanManageHotel(
            isAdmin,
            hotel.HotelManagers.Any(hm => hm.UserId == userId));

        hotel.IsActive = isActive;

        await _hotelRepository.UpdateAsync(hotel, cancellationToken);
    }

}