using Rezerv.Application.DTOs.Dashboard;

namespace Rezerv.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAdminDashboardAsync(
        CancellationToken cancellationToken = default);

    Task<DashboardDto> GetManagerDashboardAsync(
        Guid managerId,
        CancellationToken cancellationToken = default);

    Task<DashboardDto> GetCustomerDashboardAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}