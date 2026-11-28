namespace Rezerv.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default
    );

    Task DeleteImageAsync(
        string imageUrl,
        CancellationToken cancellationToken = default);
}