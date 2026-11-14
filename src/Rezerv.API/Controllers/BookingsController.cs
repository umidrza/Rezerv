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
}