namespace IRasRag.Application.Common.Interfaces.CloudFileStorage
{
    public interface ICloudFileStorageService
    {
        Task<string?> UploadAsync(
            Stream fileStream,
            string fileName,
            long fileSize,
            CancellationToken ct = default
        );
        Task<bool> DeleteAsync(string? fileUrl);
    }
}
