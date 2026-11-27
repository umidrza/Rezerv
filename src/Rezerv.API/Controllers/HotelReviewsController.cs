using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.HotelReview;
using Rezerv.Application.Interfaces;
using System.Security.Claims;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/reviews")]
public class HotelReviewsController : ControllerBase
{
    private readonly IHotelReviewService _reviewService;

    public HotelReviewsController(IHotelReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews(Guid hotelId)
    {
        var reviews = await _reviewService.GetHotelReviewsAsync(hotelId);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateReview(
        Guid hotelId,
        [FromBody] CreateHotelReviewDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var review = await _reviewService.CreateReviewAsync(
            hotelId,
            userId,
            dto);

        return CreatedAtAction(
            nameof(GetReviews),
            new { hotelId },
            review);
    }
}