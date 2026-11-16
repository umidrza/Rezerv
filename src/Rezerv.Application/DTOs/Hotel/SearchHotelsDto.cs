namespace Rezerv.Application.DTOs.Hotel;

public class SearchHotelsDto
{
    public string? City { get; set; }
    public int? StarRating { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int? NumberOfGuests { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}