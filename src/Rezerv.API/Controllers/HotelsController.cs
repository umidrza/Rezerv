using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHotel(CreateHotelDto dto)
    {
        try
        {
            var hotel = await _hotelService.CreateHotelAsync(dto);
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllHotels()
    {
        var hotels = await _hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHotel(Guid id)
    {
        var hotel = await _hotelService.GetHotelAsync(id);

        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchHotels(
        [FromQuery] SearchHotelsDto dto)
    {
        try
        {
            var hotels = await _hotelService.SearchHotelsAsync(dto);

            return Ok(hotels);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-hotels")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> GetMyHotels()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        var hotels = await _hotelService.GetMyHotelsAsync(userId, isAdmin);

        return Ok(hotels);
    }


    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> UpdateHotel(
        Guid id,
        UpdateHotelDto dto)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var hotel = await _hotelService.UpdateHotelAsync(
                id,
                dto,
                userId,
                isAdmin);

            return Ok(hotel);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPut("{id:guid}/activate")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> ActivateHotel(Guid id)
    {
        return await ChangeHotelStatus(id, true);
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> DeactivateHotel(Guid id)
    {
        return await ChangeHotelStatus(id, false);
    }

    private async Task<IActionResult> ChangeHotelStatus(
        Guid id,
        bool isActive)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _hotelService.SetHotelStatusAsync(
                id, isActive, userId, isAdmin);

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
    }

}