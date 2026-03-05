namespace IRasRag.Application.Common.Interfaces.Cloudinary
{
    public interface ICloudFileStorageService
    {
        Task<string?> UploadAsync(Stream fileStream, string fileName, long fileSize);
        Task<bool> DeleteAsync(string? fileUrl);
    }
}
