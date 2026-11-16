using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Application.DTOs.Common;


namespace Rezerv.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelImageRepository _hotelImageRepository;

    public HotelService(
    IHotelRepository hotelRepository,
    IHotelImageRepository hotelImageRepository)
    {
        _hotelRepository = hotelRepository;
        _hotelImageRepository = hotelImageRepository;
    }

    public async Task<HotelDto> CreateHotelAsync(CreateHotelDto dto)
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

        await _hotelRepository.AddAsync(hotel);

        if (!string.IsNullOrWhiteSpace(dto.PrimaryImageUrl))
        {
            var image = new HotelImage
            {
                Id = Guid.NewGuid(),
                HotelId = hotel.Id,
                ImageUrl = dto.PrimaryImageUrl
            };

            await _hotelImageRepository.AddAsync(image);

            hotel.PrimaryImageId = image.Id;
            hotel.PrimaryImage = image;

            await _hotelRepository.UpdateAsync(hotel);
        }

        return MapToDto(hotel);
    }

    public async Task<HotelDto?> GetHotelAsync(Guid id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);

        return hotel == null ? null : MapToDto(hotel);
    }

    public async Task<List<HotelDto>> GetAllHotelsAsync()
    {
        var hotels = await _hotelRepository.GetAllAsync();

        return hotels.Select(MapToDto).ToList();
    }

    private static HotelDto MapToDto(Hotel hotel)
    {
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            StarRating = hotel.StarRating,
            PrimaryImageUrl = hotel.PrimaryImage?.ImageUrl
        };
    }

    public async Task<PaginatedResultDto<HotelDto>> SearchHotelsAsync(SearchHotelsDto dto)
    {
        // Validate search filters
        if (dto.StarRating.HasValue &&
            (dto.StarRating < 1 || dto.StarRating > 5))
        {
            throw new ArgumentException("Star rating must be between 1 and 5.");
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

        var hotels = await _hotelRepository.SearchAsync(dto);
        var totalCount = await _hotelRepository.CountSearchResultsAsync(dto);

        var items = hotels.Select(hotel => new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            StarRating = hotel.StarRating,
            PrimaryImageUrl = hotel.PrimaryImage?.ImageUrl
        }).ToList();

        return new PaginatedResultDto<HotelDto>
        {
            Items = items,
            Page = dto.Page,
            PageSize = dto.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize)
        };
    }
}