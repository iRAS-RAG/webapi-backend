namespace IRasRag.Application.Common.Interfaces.FileExtraction
{
    public interface IFileTextExtractor
    {
        string ExtractText(Stream fileStream);
        bool CanHandle(string fileExtension);
    }
}
