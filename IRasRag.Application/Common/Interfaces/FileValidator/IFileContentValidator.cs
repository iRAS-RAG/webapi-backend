namespace IRasRag.Application.Common.Interfaces.FileValidator
{
    public interface IFileContentValidator
    {
        //bool HasValidSignature(Stream stream, string fileName);
        bool HasValidSize(long fileSize);
        //bool HasValidExtension(string fileName);
        string? DetectExtension(Stream stream);
    }
}
