using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var booking = await _bookingService.CreateBookingAsync(
                dto,
                userId.Value);

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = booking.Id },
                booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var booking = await _bookingService.GetBookingForUserAsync(
                id,
                userId.Value,
                User.IsInRole("Admin"));

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("check-availability")]
    public async Task<IActionResult> CheckRoomAvailability(
        [FromQuery] Guid roomId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut)
    {
        try
        {
            var isAvailable = await _bookingService.CheckRoomAvailabilityAsync(
                roomId,
                checkIn,
                checkOut);

            return Ok(new { isAvailable });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var bookings = await _bookingService.GetMyBookingsAsync(userId.Value);

        return Ok(bookings);
    }

    [Authorize(Roles = "Admin,HotelManager")]
    [HttpGet("hotel/{hotelId:guid}")]
    public async Task<IActionResult> GetHotelBookings(Guid hotelId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var bookings = await _bookingService.GetHotelBookingsAsync(
                hotelId,
                userId.Value,
                isAdmin);

            return Ok(bookings);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Admin,HotelManager")]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveBooking(Guid id)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _bookingService.ApproveBookingAsync(
                id,
                userId.Value,
                User.IsInRole("Admin"));

            if (!result)
                return BadRequest(new { message = "Booking could not be approved." });

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Admin,HotelManager")]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectBooking(Guid id)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _bookingService.RejectBookingAsync(
                id,
                userId.Value,
                User.IsInRole("Admin"));

            if (!result)
                return BadRequest(new { message = "Booking could not be rejected." });

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Customer")]
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            await _bookingService.CancelBookingAsync(id, userId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var id) ? id : null;
    }
}