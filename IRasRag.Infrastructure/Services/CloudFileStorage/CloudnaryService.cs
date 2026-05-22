using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hangfire;
using IRasRag.Application.Common.Interfaces.CloudFileStorage;
using IRasRag.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.CloudFileStorage
{
    public class CloudnaryService : ICloudFileStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudnaryService> _logger;
        private readonly CloudinarySettings _settings;

        public CloudnaryService(
            IOptions<CloudinarySettings> settings,
            ILogger<CloudnaryService> logger
        )
        {
            _settings = settings.Value;
            _logger = logger;
            _cloudinary = new Cloudinary(
                new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret)
            );
            _cloudinary.Api.Secure = true; //use HTTPS
        }

        public async Task<string?> UploadAsync(Stream fileStream, string fileName, long fileSize)
        {
            try
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = _settings.DocumentFolderPath,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false,
                };

                var result = await _cloudinary.UploadLargeRawAsync(uploadParams);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Message}", result.Error.Message);
                    throw new InvalidOperationException("Upload to cloud storage failed.");
                }

                _logger.LogInformation(
                    "File '{FileName}' uploaded successfully. URL: {Url}",
                    fileName,
                    result.SecureUrl
                );

                return result.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error uploading file '{FileName}'.", fileName);
                throw;
            }
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 150, 300 })]
        public async Task<bool> DeleteAsync(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                _logger.LogWarning("Delete attempted with null or empty URL.");
                return false;
            }

            try
            {
                var publicId = ExtractPublicId(fileUrl);
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("Could not extract public ID from URL: {Url}", fileUrl);
                    return false;
                }

                var deleteParams = new DeletionParams(publicId) { ResourceType = ResourceType.Raw };

                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok" || result.Result == "not found")
                {
                    _logger.LogInformation(
                        "File with public ID '{PublicId}' deleted (result: {Result}).",
                        publicId,
                        result.Result
                    );
                    return true;
                }

                _logger.LogWarning(
                    "Cloudinary delete returned unexpected result '{Result}' for public ID '{PublicId}'.",
                    result.Result,
                    publicId
                );
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file with URL '{Url}'.", fileUrl);
                return false;
            }
        }

        /// <summary>
        /// Extracts the Cloudinary public ID from a secure URL.
        /// Expected format: https://res.cloudinary.com/{cloud}/raw/upload/v{version}/{public_id}
        /// </summary>
        private static string ExtractPublicId(string fileUrl)
        {
            const string uploadSegment = "/upload/";
            var uploadIndex = fileUrl.IndexOf(uploadSegment, StringComparison.OrdinalIgnoreCase);
            if (uploadIndex < 0)
                return string.Empty;

            var afterUpload = fileUrl[(uploadIndex + uploadSegment.Length)..];

            // Strip optional version prefix, e.g. "v1234567890/"
            if (afterUpload.Length > 1 && afterUpload[0] == 'v' && char.IsDigit(afterUpload[1]))
            {
                var slashIndex = afterUpload.IndexOf('/');
                if (slashIndex > 0)
                    afterUpload = afterUpload[(slashIndex + 1)..];
            }

            // Strip query string if present
            var queryIndex = afterUpload.IndexOf('?');
            if (queryIndex >= 0)
                afterUpload = afterUpload[..queryIndex];

            return afterUpload;
        }
    }
}
