namespace Rezerv.Application.DTOs.Hotel;

public class SearchHotelsDto
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public int? StarRating { get; set; }
    public double? MinimumAverageRating { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;

    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int? NumberOfGuests { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}