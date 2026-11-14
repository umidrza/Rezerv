using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Hotel;
using Rezerv.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
}