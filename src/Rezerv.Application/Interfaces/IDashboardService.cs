using Rezerv.Application.DTOs.Dashboard;

namespace Rezerv.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAdminDashboardAsync();

    Task<DashboardDto> GetManagerDashboardAsync(Guid managerId);

    Task<DashboardDto> GetCustomerDashboardAsync(Guid userId);
}