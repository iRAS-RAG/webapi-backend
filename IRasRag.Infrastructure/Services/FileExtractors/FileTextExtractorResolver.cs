using IRasRag.Application.Common.Interfaces.FileExtraction;
using IRasRag.Application.Common.Interfaces.FileExtractor;

namespace IRasRag.Infrastructure.Services.FileExtractors
{
    public class FileTextExtractorResolver : IFileTextExtractorResolver
    {
        private readonly IEnumerable<IFileTextExtractor> _extractors;

        public FileTextExtractorResolver(IEnumerable<IFileTextExtractor> extractors)
        {
            _extractors = extractors;
        }

        public string ExtractText(Stream fileStream, string fileExtension)
        {
            var extension = fileExtension.ToLowerInvariant();
            var extractor = _extractors.FirstOrDefault(e => e.CanHandle(extension));

            if (extractor is null)
                return string.Empty;

            return extractor.ExtractText(fileStream);
        }
    }
}
