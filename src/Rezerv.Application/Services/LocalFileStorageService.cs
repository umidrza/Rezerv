using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rezerv.Application.Constants;
using Rezerv.Application.Interfaces;
using SkiaSharp;

namespace Rezerv.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;

    private static readonly Dictionary<string, string> AllowedImageTypes = new()
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    private const int CopyBufferSize = 81920;

    private string UploadsFolder => Path.Combine(
        _environment.ContentRootPath,
        "wwwroot",
        "uploads",
        "hotels");

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        ILogger<LocalFileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveImageAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

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

        await using var imageContent = new MemoryStream();
        await CopyWithinLimitAsync(fileStream, imageContent);

        if (imageContent.Length == 0)
            throw new ArgumentException("Image file cannot be empty.");

        if (!HasExpectedSignature(imageContent, extension))
            throw new ArgumentException("Image file contents do not match the file type.");

        var imageBytes = imageContent.ToArray();
        using (var codecStream = new MemoryStream(imageBytes, writable: false))
        {
            using var codec = SKCodec.Create(codecStream);
            if (codec is null)
                throw new ArgumentException("Invalid image file.");

            if (codec.EncodedFormat != GetExpectedFormat(extension))
            {
                throw new ArgumentException(
                    "Image file contents do not match the file type.");
            }

            var imageInfo = codec.Info;
            if (imageInfo.Width > ImageUploadLimits.MaxDimensionPixels ||
                imageInfo.Height > ImageUploadLimits.MaxDimensionPixels ||
                (long)imageInfo.Width * imageInfo.Height >
                    ImageUploadLimits.MaxPixelCount)
            {
                throw new ArgumentException(
                    $"Image dimensions cannot exceed {ImageUploadLimits.MaxDimensionPixels} pixels per side or " +
                    $"{ImageUploadLimits.MaxPixelCount:N0} total pixels.");
            }

            using var decodeStream = new MemoryStream(imageBytes, writable: false);
            using var decodedImage = SKBitmap.Decode(decodeStream);
            if (decodedImage is null ||
                decodedImage.Width != imageInfo.Width ||
                decodedImage.Height != imageInfo.Height)
            {
                throw new ArgumentException("Unsupported or invalid image file.");
            }
        }

        Directory.CreateDirectory(UploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(UploadsFolder, uniqueFileName);

        try
        {
            await using var outputStream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                CopyBufferSize,
                FileOptions.Asynchronous);

            imageContent.Position = 0;
            await imageContent.CopyToAsync(outputStream);
        }
        catch
        {
            TryDeleteFile(filePath);
            throw;
        }

        return $"/uploads/hotels/{uniqueFileName}";
    }

    public Task DeleteImageAsync(string imageUrl)
    {
        var fileName = Path.GetFileName(imageUrl);

        if (string.IsNullOrWhiteSpace(fileName))
            return Task.CompletedTask;

        var filePath = Path.Combine(UploadsFolder, fileName);
        File.Delete(filePath);

        return Task.CompletedTask;
    }

    private static async Task CopyWithinLimitAsync(
        Stream source,
        Stream destination)
    {
        var buffer = new byte[CopyBufferSize];
        int bytesRead;

        while ((bytesRead = await source.ReadAsync(buffer.AsMemory())) > 0)
        {
            if (destination.Length + bytesRead > ImageUploadLimits.MaxFileSizeBytes)
                throw new ArgumentException("Image size cannot exceed 5 MB.");

            await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
        }
    }

    private static bool HasExpectedSignature(
        MemoryStream imageContent,
        string extension)
    {
        var bytes = imageContent.GetBuffer();
        var length = (int)imageContent.Length;

        return extension switch
        {
            ".jpg" or ".jpeg" =>
                length >= 3 &&
                bytes[0] == 0xFF &&
                bytes[1] == 0xD8 &&
                bytes[2] == 0xFF,
            ".png" =>
                length >= 8 &&
                bytes.AsSpan(0, 8).SequenceEqual(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" =>
                length >= 12 &&
                bytes.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
                bytes.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            _ => false
        };
    }

    private static SKEncodedImageFormat GetExpectedFormat(string extension)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            ".png" => SKEncodedImageFormat.Png,
            ".webp" => SKEncodedImageFormat.Webp,
            _ => throw new ArgumentException("Unsupported image format.")
        };
    }

    private void TryDeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(
                ex,
                "Could not remove incomplete hotel image file {FilePath}.",
                filePath);
        }
    }
}
