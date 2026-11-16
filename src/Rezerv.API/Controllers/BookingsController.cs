using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Booking;
using Rezerv.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Rezerv.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        try
        {
            var booking = await _bookingService.CreateBookingAsync(dto, userId);

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var booking = await _bookingService.GetBookingAsync(id, userId);

        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [AllowAnonymous]
    [HttpPost("check-availability")]
    public async Task<IActionResult> CheckRoomAvailability(CheckRoomAvailabilityDto dto)
    {
        try
        {
            var isAvailable = await _bookingService.CheckRoomAvailabilityAsync(dto);

            return Ok(new { isAvailable });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> ApproveBooking(Guid id)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _bookingService.ApproveBookingAsync(id, userId, isAdmin);
            return Ok(new { message = "Booking approved successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> RejectBooking(Guid id)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _bookingService.RejectBookingAsync(id, userId, isAdmin);
            return Ok(new { message = "Booking rejected successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await _bookingService.CancelBookingAsync(id, userId);

            return Ok(new { message = "Booking cancelled successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("hotel/{hotelId:guid}")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> GetHotelBookings(Guid hotelId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var bookings = await _bookingService.GetHotelBookingsAsync(
                hotelId,
                userId,
                isAdmin);

            return Ok(bookings);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var bookings = await _bookingService.GetMyBookingsAsync(userId);

        return Ok(bookings);
    }
}