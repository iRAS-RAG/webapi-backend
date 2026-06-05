namespace IRasRag.Application.Common.Interfaces.BackgroundJobs
{
    public interface IDocumentIngestJob
    {
        Task RunAsync(Guid documentId);
        Task MarkAsFailedAsync(Guid documentId);
    }
}
