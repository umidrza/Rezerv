using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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
    public async Task<IActionResult> CreateHotel(
        CreateHotelDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var hotel = await _hotelService.CreateHotelAsync(
                dto,
                cancellationToken);
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllHotels(
        CancellationToken cancellationToken)
    {
        var hotels = await _hotelService.GetAllHotelsAsync(cancellationToken);
        return Ok(hotels);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHotel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelService.GetHotelAsync(id, cancellationToken);

        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchHotels(
        [FromQuery] SearchHotelsDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var hotels = await _hotelService.SearchHotelsAsync(
                dto,
                cancellationToken);

            return Ok(hotels);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-hotels")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> GetMyHotels(
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        var hotels = await _hotelService.GetMyHotelsAsync(
            userId,
            isAdmin,
            cancellationToken);

        return Ok(hotels);
    }


    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> UpdateHotel(
        Guid id,
        UpdateHotelDto dto,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var hotel = await _hotelService.UpdateHotelAsync(
                id,
                dto,
                userId,
                isAdmin,
                cancellationToken);

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
    public async Task<IActionResult> ActivateHotel(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ChangeHotelStatus(id, true, cancellationToken);
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> DeactivateHotel(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ChangeHotelStatus(id, false, cancellationToken);
    }

    private async Task<IActionResult> ChangeHotelStatus(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _hotelService.SetHotelStatusAsync(
                id,
                isActive,
                userId,
                isAdmin,
                cancellationToken);

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