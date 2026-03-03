namespace IRasRag.Application.Common.Interfaces.FileExtractor
{
    public interface IFileTextExtractorResolver
    {
        string ExtractText(Stream fileStream, string fileName);
    }
}
