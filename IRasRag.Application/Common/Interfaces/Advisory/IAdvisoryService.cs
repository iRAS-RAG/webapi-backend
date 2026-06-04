namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public record AdvisoryChatResult(
        string Answer,
        bool IsOffTopic,
        IReadOnlyList<string>? Citations = null,
        string? Intent = null
    );

    public record MortalityDiagnosisResult(
        string Answer,
        string Intent,
        double? Confidence,
        IReadOnlyList<string>? Citations,
        string AnswerBasis,
        Dictionary<string, object>? IotData = null
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
        Task<MortalityDiagnosisResult> DiagnoseMortalityAsync(
            Guid tankId,
            Guid userId,
            Guid? batchId = null,
            string? timeRange = null,
            string? message = null,
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
