using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.RoomType;
using Rezerv.Application.Interfaces;

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
    public async Task<IActionResult> CreateRoomType(CreateRoomTypeDto dto)
    {
        try
        {
            var roomType = await _roomTypeService.CreateRoomTypeAsync(dto);

            return CreatedAtAction(
                nameof(GetRoomType),
                new { id = roomType.Id },
                roomType);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
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