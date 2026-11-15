
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelImagesController : ControllerBase
{
    private readonly IHotelImageService _hotelImageService;

    public HotelImagesController(IHotelImageService hotelImageService)
    {
        _hotelImageService = hotelImageService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> AddImage(CreateHotelImageDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var image = await _hotelImageService.AddImageAsync(
                dto, userId, isAdmin);

            return Ok(image);
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
    public async Task<IActionResult> GetHotelImages(Guid hotelId)
    {
        var images = await _hotelImageService.GetHotelImagesAsync(hotelId);

        return Ok(images);
    }

    [HttpPut("hotel/{hotelId:guid}/primary-image/{imageId:guid}")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> SetPrimaryImage(
        Guid hotelId,
        Guid imageId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _hotelImageService.SetPrimaryImageAsync(
                hotelId,
                imageId,
                userId,
                isAdmin);

            return NoContent();
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
}
