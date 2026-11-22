
using Microsoft.AspNetCore.Hosting;
using Rezerv.Application.Interfaces;

namespace Rezerv.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    private static readonly Dictionary<string, string> AllowedImageTypes = new()
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp",
        [".gif"] = "image/gif",
        [".bmp"] = "image/bmp",
        [".avif"] = "image/avif",
        [".tif"] = "image/tiff",
        [".tiff"] = "image/tiff"
    };

    private string UploadsFolder => Path.Combine(
        _environment.ContentRootPath,
        "wwwroot",
        "uploads",
        "hotels"
    );

    private const long MaxFileSize = 5 * 1024 * 1024;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveImageAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedImageTypes.TryGetValue(extension, out var expectedContentType))
            throw new ArgumentException("Unsupported image format.");

        if (!string.Equals(
            contentType,
            expectedContentType,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid image content type.");
        }

        if (fileStream.Length > MaxFileSize)
            throw new ArgumentException("Image size cannot exceed 5 MB.");

        Directory.CreateDirectory(UploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(UploadsFolder, uniqueFileName);

        await using var outputStream = new FileStream(
            filePath,
            FileMode.CreateNew
        );

        await fileStream.CopyToAsync(outputStream);

        return $"/uploads/hotels/{uniqueFileName}";
    }

    public Task DeleteImageAsync(string imageUrl)
    {
        var fileName = Path.GetFileName(imageUrl);

        if (string.IsNullOrWhiteSpace(fileName))
            return Task.CompletedTask;

        var filePath = Path.Combine(UploadsFolder, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
