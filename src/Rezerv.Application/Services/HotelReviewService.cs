using Rezerv.Application.DTOs.HotelReview;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Services;

public class HotelReviewService : IHotelReviewService
{
    private readonly IHotelReviewRepository _reviewRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;

    public HotelReviewService(
        IHotelReviewRepository reviewRepository,
        IBookingRepository bookingRepository,
        IHotelRepository hotelRepository,
        IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
    }

    public async Task<HotelReviewDto> CreateReviewAsync(
        Guid hotelId,
        Guid userId,
        CreateHotelReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(dto.Comment))
            throw new ArgumentException("Review comment is required.");

        if (dto.Comment.Length > 1000)
            throw new ArgumentException("Review cannot exceed 1000 characters.");

        var hotel = await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Hotel not found.");

        var bookings = await _bookingRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        var hasCompletedBooking = bookings.Any(b =>
            b.HotelId == hotelId &&
            b.Status == BookingStatus.Approved &&
            b.CheckOut <= DateTime.UtcNow);

        if (!hasCompletedBooking)
            throw new InvalidOperationException(
                "You can only review a hotel after completing an approved booking.");

        if (await _reviewRepository.ExistsAsync(
            hotelId,
            userId,
            cancellationToken))
            throw new InvalidOperationException(
                "You have already reviewed this hotel.");

        var review = new HotelReview
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review, cancellationToken);

        var names = await _userRepository.GetFullNamesByIdsAsync(
            new[] { userId },
            cancellationToken);

        return MapToDto(review, names);
    }

    public async Task<List<HotelReviewDto>> GetHotelReviewsAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        if (await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken) is null)
            throw new KeyNotFoundException("Hotel not found.");

        var reviews = await _reviewRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);

        var userIds = reviews.Select(r => r.UserId);

        var names = await _userRepository.GetFullNamesByIdsAsync(
            userIds,
            cancellationToken);

        return reviews.Select(r => MapToDto(r, names)).ToList();
    }

    public async Task<bool> CanReviewHotelAsync(
        Guid hotelId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken) is null)
            return false;

        if (await _reviewRepository.ExistsAsync(
            hotelId,
            userId,
            cancellationToken))
            return false;

        var bookings = await _bookingRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        return bookings.Any(b =>
            b.HotelId == hotelId &&
            b.Status == BookingStatus.Approved &&
            b.CheckOut <= DateTime.UtcNow);
    }

    private static HotelReviewDto MapToDto(
        HotelReview review,
        Dictionary<Guid, string> names)
    {
        return new HotelReviewDto
        {
            Id = review.Id,
            HotelId = review.HotelId,
            UserId = review.UserId,
            UserName = names.GetValueOrDefault(review.UserId, "Unknown"),
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}