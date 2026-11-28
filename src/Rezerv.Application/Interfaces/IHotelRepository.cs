using Rezerv.Application.DTOs.Dashboard;
using Rezerv.Domain.Entities;
using Rezerv.Application.DTOs.Hotel;

namespace Rezerv.Application.Interfaces;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<Hotel>> GetAllAsync(
        CancellationToken cancellationToken = default);
    Task AddAsync(
        Hotel hotel,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Hotel hotel,
        CancellationToken cancellationToken = default);
    Task<List<Hotel>> SearchAsync(
        SearchHotelsDto dto,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchResultsAsync(
        SearchHotelsDto dto,
        CancellationToken cancellationToken = default);
    Task<List<Hotel>> GetByManagerIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<int> CountByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default);
    Task<int> CountActiveByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default);
    Task<HotelDashboardCounts> GetDashboardCountsAsync(
        CancellationToken cancellationToken = default);
    Task<HotelDashboardCounts> GetDashboardCountsByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default);
}