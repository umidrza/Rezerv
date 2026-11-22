using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Services;

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
        Guid userId)
    {
        if (dto.CheckIn.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Check-in date cannot be in the past.");

        if (dto.CheckOut <= dto.CheckIn)
            throw new ArgumentException("Check-out must be after check-in.");

        if (dto.NumberOfGuests <= 0)
            throw new ArgumentException("Number of guests must be greater than zero.");

        var room = await _roomRepository.GetByIdAsync(dto.RoomId);

        if (room == null || !room.IsActive)
            throw new ArgumentException("Room not found or inactive.");

        if (room.RoomType.HotelId != dto.HotelId)
            throw new ArgumentException("Room does not belong to this hotel.");

        if (dto.NumberOfGuests > room.RoomType.Capacity)
            throw new ArgumentException("Room capacity exceeded.");

        var checkIn = DateTime.SpecifyKind(dto.CheckIn, DateTimeKind.Utc);
        var checkOut = DateTime.SpecifyKind(dto.CheckOut, DateTimeKind.Utc);

        var isAvailable = await _roomRepository.IsRoomAvailableAsync(
            dto.RoomId,
            checkIn,
            checkOut);

        if (!isAvailable)
            throw new ArgumentException("Room is not available for these dates.");

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

        await _bookingRepository.AddAsync(booking);

        return await GetBookingAsync(booking.Id)
            ?? throw new InvalidOperationException("Booking could not be retrieved.");
    }

    public async Task<BookingDto?> GetBookingAsync(Guid bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        return booking == null ? null : MapToDto(booking);
    }

    public async Task<bool> CheckRoomAvailabilityAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut)
    {
        if (checkIn.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Check-in date cannot be in the past.");

        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out must be after check-in.");

        var room = await _roomRepository.GetByIdAsync(roomId);

        if (room == null || !room.IsActive)
            return false;

        checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
        checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

        return await _roomRepository.IsRoomAvailableAsync(
            roomId,
            checkIn,
            checkOut);
    }

    public async Task<bool> ApproveBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return false;

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository.IsManagerAssignedAsync(
                userId,
                booking.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You cannot manage this hotel's bookings.");
        }

        if (booking.Status != BookingStatus.Submitted)
            return false;

        return await _bookingRepository.ApproveAsync(bookingId);
    }

    public async Task<bool> RejectBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return false;

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository.IsManagerAssignedAsync(
                userId,
                booking.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You cannot manage this hotel's bookings.");
        }

        if (booking.Status != BookingStatus.Submitted)
            return false;

        booking.Status = BookingStatus.Rejected;

        await _bookingRepository.UpdateAsync(booking);

        return true;
    }

    public async Task<List<BookingDto>> GetHotelBookingsAsync(
        Guid hotelId,
        Guid userId,
        bool isAdmin)
    {
        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository.IsManagerAssignedAsync(
                userId,
                hotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException("You cannot access this hotel's bookings.");
        }

        var bookings = await _bookingRepository.GetByHotelIdAsync(hotelId);

        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<BookingDto>> GetMyBookingsAsync(Guid userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);

        return bookings.Select(MapToDto).ToList();
    }

    public async Task<bool> CancelBookingAsync(
        Guid bookingId,
        Guid userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null || booking.UserId != userId)
            return false;

        if (booking.Status != BookingStatus.Submitted &&
            booking.Status != BookingStatus.Approved)
            return false;

        booking.Status = BookingStatus.Cancelled;

        await _bookingRepository.UpdateAsync(booking);

        return true;
    }

    public async Task<BookingDto?> GetBookingForUserAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        if (!isAdmin && booking.UserId != userId)
        {
            var isAssigned = await _hotelManagerRepository.IsManagerAssignedAsync(
                userId,
                booking.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You cannot access this booking.");
        }

        return MapToDto(booking);
    }

    private static BookingDto MapToDto(Booking booking)
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