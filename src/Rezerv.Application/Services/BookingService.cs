using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
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
}