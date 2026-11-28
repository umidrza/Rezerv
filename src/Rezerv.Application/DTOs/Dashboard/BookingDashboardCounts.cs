namespace Rezerv.Application.DTOs.Dashboard;

public sealed record BookingDashboardCounts(
    int TotalBookings,
    int PendingBookings,
    int ApprovedBookings);
