namespace IRasRag.Application.Common.Interfaces.BackgroundJobs
{
    public interface IDocumentRagDeleteJob
    {
        Task RunAsync(string fileUrl);
    }
}
