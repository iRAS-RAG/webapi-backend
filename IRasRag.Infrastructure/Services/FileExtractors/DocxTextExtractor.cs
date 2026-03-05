using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using IRasRag.Application.Common.Interfaces.FileExtraction;

namespace IRasRag.Infrastructure.Services.FileExtractors
{
    public class DocxTextExtractor : IFileTextExtractor
    {
        public bool CanHandle(string fileExtension) =>
            fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

        public string ExtractText(Stream fileStream)
        {
            using var doc = WordprocessingDocument.Open(fileStream, isEditable: false);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body is null)
                return string.Empty;

            return string.Join(
                Environment.NewLine,
                body.Descendants<Paragraph>()
                    .Select(p => p.InnerText)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
            );
        }
    }
}
