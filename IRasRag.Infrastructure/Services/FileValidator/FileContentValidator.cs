using System.IO.Compression;
using System.Text;
using IRasRag.Application.Common.Interfaces.FileValidator;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.FileValidator
{
    public class FileContentValidator : IFileContentValidator
    {
        private readonly ILogger<FileContentValidator> _logger;

        public FileContentValidator(ILogger<FileContentValidator> logger)
        {
            _logger = logger;
        }

        private static readonly byte[] _pdfSignature = { 0x25, 0x50, 0x44, 0x46 };
        private static readonly byte[] _zipSignature = { 0x50, 0x4B, 0x03, 0x04 };

        private const long MaxFileSizeBytes = 50L * 1024 * 1024; // 50 MB

        public bool HasValidSize(long fileSize)
        {
            if (fileSize <= 0 || fileSize > MaxFileSizeBytes)
            {
                return false;
            }
            return true;
        }

        public string? DetectExtension(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            var headerBytes = reader.ReadBytes(4);
            stream.Position = 0;

            if (_pdfSignature.SequenceEqual(headerBytes.Take(4)))
                return ".pdf";

            // 2. ZIP-based detection
            if (headerBytes.SequenceEqual(_zipSignature))
            {
                stream.Position = 0;
                try
                {
                    using var archive = new ZipArchive(
                        stream,
                        ZipArchiveMode.Read,
                        leaveOpen: true
                    );

                    // True DOCX must contain Word main document part
                    if (
                        archive.GetEntry("word/document.xml") != null
                        && archive.GetEntry("[Content_Types].xml") != null
                    )
                    {
                        return ".docx";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to inspect ZIP archive for DOCX detection");
                    return null;
                }
                finally
                {
                    stream.Position = 0; // Reset after ZIP inspection
                }
            }
            stream.Position = 0;
            return null;
        }
    }
}
