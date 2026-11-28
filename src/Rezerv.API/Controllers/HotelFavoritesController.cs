using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize(Roles = "Customer")]
public class HotelFavoritesController : ControllerBase
{
    private readonly IHotelFavoriteService _favoriteService;

    public HotelFavoritesController(IHotelFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites()
    {
        var favorites = await _favoriteService.GetMyFavoritesAsync(GetUserId());

        return Ok(favorites);
    }

    [HttpPost("{hotelId:guid}")]
    public async Task<IActionResult> AddFavorite(Guid hotelId)
    {
        await _favoriteService.AddFavoriteAsync(GetUserId(), hotelId);

        return NoContent();
    }

    [HttpDelete("{hotelId:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid hotelId)
    {
        await _favoriteService.RemoveFavoriteAsync(GetUserId(), hotelId);

        return NoContent();
    }

    [HttpGet("{hotelId:guid}/status")]
    public async Task<IActionResult> IsFavorite(Guid hotelId)
    {
        var isFavorite = await _favoriteService.IsFavoriteAsync(
            GetUserId(),
            hotelId);

        return Ok(isFavorite);
    }
}