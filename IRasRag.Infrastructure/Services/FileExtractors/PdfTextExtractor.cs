using IRasRag.Application.Common.Interfaces.FileExtraction;
using UglyToad.PdfPig;

namespace IRasRag.Infrastructure.Services.TextExtractors
{
    public class PdfTextExtractor : IFileTextExtractor
    {
        public bool CanHandle(string fileExtension) =>
            fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);

        public string ExtractText(Stream pdfStream)
        {
            using var document = PdfDocument.Open(pdfStream);

            var text = string.Join(
                Environment.NewLine,
                document.GetPages().Select(p => p.Text));
            return text;
        }
    }
}
