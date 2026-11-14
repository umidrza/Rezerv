using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Room;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomManagementService _roomService;

    public RoomsController(IRoomManagementService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
    {
        try
        {
            var room = await _roomService.CreateRoomAsync(dto);

            return CreatedAtAction(
                nameof(GetRoom),
                new { id = room.Id },
                room);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _roomService.GetRoomAsync(id);

        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("roomtype/{roomTypeId:guid}")]
    public async Task<IActionResult> GetRoomsByRoomType(Guid roomTypeId)
    {
        var rooms = await _roomService.GetRoomsByRoomTypeAsync(roomTypeId);

        return Ok(rooms);
    }
}