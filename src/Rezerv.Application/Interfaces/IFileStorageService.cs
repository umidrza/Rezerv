namespace Rezerv.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(
        Stream fileStream,
        string fileName,
        string contentType
    );

    Task DeleteImageAsync(string imageUrl);
}