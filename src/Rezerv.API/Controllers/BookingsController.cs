using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ApiControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        CreateBookingDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var booking = await _bookingService.CreateBookingAsync(
            dto,
            userId,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetBooking),
            new { id = booking.Id },
            booking);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var booking = await _bookingService.GetBookingForUserAsync(
            id,
            userId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpGet("check-availability")]
    public async Task<IActionResult> CheckRoomAvailability(
        [FromQuery] CheckRoomAvailabilityDto request,
        CancellationToken cancellationToken)
    {
        var isAvailable = await _bookingService.CheckRoomAvailabilityAsync(
            request.RoomId,
            request.CheckIn,
            request.CheckOut,
            cancellationToken);

        return Ok(new { isAvailable });
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings(
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var bookings = await _bookingService.GetMyBookingsAsync(
            userId,
            cancellationToken);

        return Ok(bookings);
    }

    [Authorize(Roles = "Admin,HotelManager")]
    [HttpGet("hotel/{hotelId:guid}")]
    public async Task<IActionResult> GetHotelBookings(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        var bookings = await _bookingService.GetHotelBookingsAsync(
            hotelId,
            userId,
            isAdmin,
            cancellationToken);

        return Ok(bookings);
    }

    [Authorize(Roles = "Admin,HotelManager")]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveBooking(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _bookingService.ApproveBookingAsync(
            id,
            userId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result)
            return BadRequest(new { message = "Booking could not be approved." });

        return NoContent();
    }

    [Authorize(Roles = "Admin,HotelManager")]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectBooking(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _bookingService.RejectBookingAsync(
            id,
            userId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result)
            return BadRequest(new { message = "Booking could not be rejected." });

        return NoContent();
    }

    [Authorize(Roles = "Customer")]
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _bookingService.CancelBookingAsync(
            id,
            userId,
            cancellationToken);
        return NoContent();
    }
}