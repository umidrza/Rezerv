using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.Constants;
using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/hotel-images")]
public class HotelImagesController : ApiControllerBase
{
    private readonly IHotelImageService _hotelImageService;

    public HotelImagesController(IHotelImageService hotelImageService)
    {
        _hotelImageService = hotelImageService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HotelManager")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(ImageUploadLimits.MaxMultipartRequestSizeBytes)]
    [RequestFormLimits(
        MultipartBodyLengthLimit = ImageUploadLimits.MaxMultipartRequestSizeBytes)]
    public async Task<IActionResult> AddImage(
        [FromForm] CreateHotelImageDto dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");

            var result = await _hotelImageService.AddImageAsync(
                dto, userId, isAdmin);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
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

    [HttpPut("{hotelId:guid}/{imageId:guid}/primary")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> SetPrimaryImage(
        Guid hotelId,
        Guid imageId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");

            await _hotelImageService.SetPrimaryImageAsync(
                hotelId, imageId, userId, isAdmin);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{hotelId:guid}/{imageId:guid}")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> DeleteImage(
        Guid hotelId,
        Guid imageId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");

            await _hotelImageService.DeleteImageAsync(
                hotelId, imageId, userId, isAdmin);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}