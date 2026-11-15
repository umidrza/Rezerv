using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;
using System.Linq;

namespace Rezerv.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelManagerRepository _hotelManagerRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        IHotelManagerRepository hotelManagerRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _hotelManagerRepository = hotelManagerRepository;
    }

    public async Task<BookingDto> CreateBookingAsync(
        CreateBookingDto dto,
        Guid userId)
    {
        if (dto.CheckIn >= dto.CheckOut)
            throw new ArgumentException("Invalid booking dates.");

        if (dto.NumberOfGuests <= 0)
            throw new ArgumentException("Number of guests must be greater than zero.");

        if (dto.RoomIds == null || dto.RoomIds.Count == 0)
            throw new ArgumentException("At least one room must be selected.");

        if (dto.RoomIds.Distinct().Count() != dto.RoomIds.Count)
            throw new ArgumentException("Duplicate rooms are not allowed.");

        var rooms = await _roomRepository.GetByIdsAsync(dto.RoomIds);

        if (rooms.Count != dto.RoomIds.Count)
            throw new ArgumentException("One or more rooms were not found.");

        if (rooms.Any(room => !room.IsActive || room.RoomType.HotelId != dto.HotelId))
            throw new ArgumentException("Selected rooms are invalid.");

        var capacity = rooms.Sum(room => room.RoomType.Capacity);

        if (capacity < dto.NumberOfGuests)
            throw new ArgumentException("Selected rooms cannot accommodate all guests.");

        var available = await _roomRepository.AreRoomsAvailableAsync(
            dto.RoomIds,
            dto.CheckIn,
            dto.CheckOut);

        if (!available)
            throw new ArgumentException("One or more rooms are unavailable.");

        var nights = (dto.CheckOut - dto.CheckIn).Days;

        var totalPrice = rooms.Sum(
            room => room.RoomType.PricePerNight * nights);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HotelId = dto.HotelId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            NumberOfGuests = dto.NumberOfGuests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            BookingRooms = rooms.Select(room => new BookingRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                PricePerNight = room.RoomType.PricePerNight
            }).ToList()
        };

        await _bookingRepository.AddAsync(booking);

        return new BookingDto
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status.ToString()
        };
    }

    public async Task<BookingDto?> GetBookingAsync(Guid id, Guid userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null || booking.UserId != userId)
            return null;

        return new BookingDto
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status.ToString()
        };
    }

    public async Task<bool> CheckRoomAvailabilityAsync(CheckRoomAvailabilityDto dto)
    {
        if (dto.CheckIn >= dto.CheckOut)
            throw new ArgumentException("Check-out must be after check-in.");

        if (dto.RoomIds == null || dto.RoomIds.Count == 0)
            throw new ArgumentException("Select at least one room.");

        if (dto.RoomIds.Distinct().Count() != dto.RoomIds.Count)
            throw new ArgumentException("Duplicate room IDs are not allowed.");

        var rooms = await _roomRepository.GetByIdsAsync(dto.RoomIds);

        if (rooms.Count != dto.RoomIds.Count)
            throw new ArgumentException("One or more rooms were not found.");

        if (rooms.Any(r => !r.IsActive))
            throw new ArgumentException("One or more rooms are inactive.");

        return await _roomRepository.AreRoomsAvailableAsync(
            dto.RoomIds,
            DateTime.SpecifyKind(dto.CheckIn, DateTimeKind.Utc),
            DateTime.SpecifyKind(dto.CheckOut, DateTimeKind.Utc));
    }

    public async Task ApproveBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            throw new ArgumentException("Booking not found.");

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository
                .IsManagerAssignedAsync(userId, booking.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this hotel.");
        }

        if (booking.Status != BookingStatus.Submitted)
            throw new ArgumentException("Only submitted bookings can be approved.");

        var roomIds = booking.BookingRooms
            .Select(br => br.RoomId)
            .ToList();

        var isAvailable = await _roomRepository.AreRoomsAvailableAsync(
            roomIds,
            booking.CheckIn,
            booking.CheckOut);

        if (!isAvailable)
            throw new ArgumentException("One or more rooms are no longer available.");

        booking.Status = BookingStatus.Approved;

        await _bookingRepository.UpdateAsync(booking);
    }

    public async Task RejectBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            throw new ArgumentException("Booking not found.");

        if (!isAdmin)
        {
            var isAssigned = await _hotelManagerRepository
                .IsManagerAssignedAsync(userId, booking.HotelId);

            if (!isAssigned)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this hotel.");
        }

        if (booking.Status != BookingStatus.Submitted)
            throw new ArgumentException("Only submitted bookings can be rejected.");

        booking.Status = BookingStatus.Rejected;

        await _bookingRepository.UpdateAsync(booking);
    }

    public async Task<List<BookingDto>> GetHotelBookingsAsync(
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

        var bookings = await _bookingRepository.GetByHotelIdAsync(hotelId);

        return bookings.Select(booking => new BookingDto
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status.ToString()
        }).ToList();
    }
}
