using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.RoomType;
using Rezerv.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypesController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> CreateRoomType(CreateRoomTypeDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var roomType = await _roomTypeService.CreateRoomTypeAsync(
                dto,
                userId,
                isAdmin);

            return Ok(roomType);
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoomType(Guid id)
    {
        var roomType = await _roomTypeService.GetRoomTypeAsync(id);

        if (roomType == null)
            return NotFound();

        return Ok(roomType);
    }

    [HttpGet("hotel/{hotelId:guid}")]
    public async Task<IActionResult> GetRoomTypesByHotel(Guid hotelId)
    {
        var roomTypes = await _roomTypeService.GetRoomTypesByHotelAsync(hotelId);

        return Ok(roomTypes);
    }
}