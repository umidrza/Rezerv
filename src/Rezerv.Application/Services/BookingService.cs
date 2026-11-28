using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;
using Rezerv.Application.Security;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Services;

/// <summary>
/// New bookings start as Submitted. Submitted bookings may become Approved,
/// Rejected, or Cancelled; Approved bookings may become Cancelled. Rejected
/// and Cancelled are terminal states. Submitted and Approved bookings block
/// room availability for overlapping dates; Draft bookings do not.
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IHotelManagerRepository hotelManagerRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _hotelManagerRepository = hotelManagerRepository;
    }

    public async Task<BookingDto> CreateBookingAsync(
        CreateBookingDto dto,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (dto.CheckIn.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Check-in date cannot be in the past.");

        if (dto.CheckOut <= dto.CheckIn)
            throw new ArgumentException("Check-out must be after check-in.");

        if (dto.NumberOfGuests <= 0)
            throw new ArgumentException("Number of guests must be greater than zero.");

        var room = await _roomRepository.GetByIdAsync(
            dto.RoomId,
            cancellationToken);

        if (room == null || !room.IsActive)
            throw new ArgumentException("Room not found or inactive.");

        if (room.RoomType.HotelId != dto.HotelId)
            throw new ArgumentException("Room does not belong to this hotel.");

        if (dto.NumberOfGuests > room.RoomType.Capacity)
            throw new ArgumentException("Room capacity exceeded.");

        var checkIn = DateTime.SpecifyKind(dto.CheckIn, DateTimeKind.Utc);
        var checkOut = DateTime.SpecifyKind(dto.CheckOut, DateTimeKind.Utc);

        var pricePerNight = room.RoomType.PricePerNight;
        var nights = (checkOut.Date - checkIn.Date).Days;
        var totalPrice = pricePerNight * nights;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HotelId = dto.HotelId,
            RoomId = dto.RoomId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            NumberOfGuests = dto.NumberOfGuests,
            PricePerNight = pricePerNight,
            TotalPrice = totalPrice,
            Status = BookingStatus.Submitted,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddIfAvailableAsync(booking, cancellationToken);

        return await GetBookingAsync(booking.Id, cancellationToken)
            ?? throw new InvalidOperationException("Booking could not be retrieved.");
    }

    public async Task<BookingDto?> GetBookingAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(
            bookingId,
            cancellationToken);

        return booking == null ? null : MapToDto(booking);
    }

    public async Task<bool> CheckRoomAvailabilityAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut,
        CancellationToken cancellationToken = default)
    {
        if (checkIn.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Check-in date cannot be in the past.");

        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out must be after check-in.");

        var room = await _roomRepository.GetByIdAsync(
            roomId,
            cancellationToken);

        if (room == null || !room.IsActive)
            return false;

        checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
        checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

        return await _roomRepository.IsRoomAvailableAsync(
            roomId,
            checkIn,
            checkOut,
            cancellationToken);
    }

    public async Task<bool> ApproveBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(
            bookingId,
            cancellationToken);

        if (booking == null)
            return false;

        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            booking.HotelId,
            isAdmin,
            cancellationToken);

        if (booking.Status != BookingStatus.Submitted)
            return false;

        return await _bookingRepository.ApproveAsync(
            bookingId,
            cancellationToken);
    }

    public async Task<bool> RejectBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(
            bookingId,
            cancellationToken);

        if (booking == null)
            return false;

        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            booking.HotelId,
            isAdmin,
            cancellationToken);

        if (booking.Status != BookingStatus.Submitted)
            return false;

        booking.Status = BookingStatus.Rejected;

        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        return true;
    }

    public async Task<List<BookingDto>> GetHotelBookingsAsync(
        Guid hotelId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await HotelManagerAuthorization.EnsureCanManageHotelAsync(
            _hotelManagerRepository,
            userId,
            hotelId,
            isAdmin,
            cancellationToken);

        var bookings = await _bookingRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);

        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<BookingDto>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        return bookings.Select(MapToDto).ToList();
    }

    public async Task CancelBookingAsync(
        Guid bookingId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(
            bookingId,
            cancellationToken);

        if (booking == null)
            throw new KeyNotFoundException("Booking not found.");

        if (booking.UserId != userId)
            throw new UnauthorizedAccessException(
                "You cannot cancel this booking.");

        if (booking.Status != BookingStatus.Submitted &&
            booking.Status != BookingStatus.Approved)
            throw new InvalidOperationException(
                "Booking cannot be cancelled in its current state.");

        booking.Status = BookingStatus.Cancelled;

        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }

    public async Task<BookingDto?> GetBookingForUserAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(
            bookingId,
            cancellationToken);

        if (booking == null)
            return null;

        if (!isAdmin && booking.UserId != userId)
        {
            await HotelManagerAuthorization.EnsureCanManageHotelAsync(
                _hotelManagerRepository,
                userId,
                booking.HotelId,
                isAdmin,
                cancellationToken);
        }

        return MapToDto(booking);
    }

    internal static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            HotelId = booking.HotelId,
            HotelName = booking.Hotel.Name,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room.RoomNumber,
            RoomTypeName = booking.Room.RoomType.Name,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            NumberOfGuests = booking.NumberOfGuests,
            PricePerNight = booking.PricePerNight,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status
        };
    }

}