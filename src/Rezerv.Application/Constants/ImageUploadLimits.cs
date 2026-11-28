namespace Rezerv.Application.Constants;

public static class ImageUploadLimits
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;
    public const int MaxDimensionPixels = 8192;
    public const long MaxPixelCount = 20_000_000;
    public const long MultipartOverheadBytes = 64 * 1024;
    public const long MaxMultipartRequestSizeBytes =
        MaxFileSizeBytes + MultipartOverheadBytes;
}
