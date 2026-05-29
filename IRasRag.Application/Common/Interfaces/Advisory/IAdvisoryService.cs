namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public record AdvisoryChatResult(
        string Answer,
        bool IsOffTopic,
        IReadOnlyList<string>? Citations = null,
        string? Intent = null
    );

    public interface IAdvisoryService
    {
        Task GenerateForAlertAsync(Guid alertId);
        Task<AdvisoryChatResult> ChatAsync(
            Guid tankId,
            Guid userId,
            string message,
            CancellationToken ct = default
        );
        Task<RagChatFeedbackResponse?> SubmitFeedbackAsync(
            Guid userId,
            string response,
            bool helpful,
            string? intent,
            string? question,
            CancellationToken ct = default
        );
    }
}
