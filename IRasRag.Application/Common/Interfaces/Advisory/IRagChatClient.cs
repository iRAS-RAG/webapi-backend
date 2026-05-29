namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public record RagChatRequest(
        string UserId,
        string FarmId,
        string TankId,
        string Species,
        string Stage,
        string Message,
        string TimeRange = "last_24h",
        bool AllowWebSearch = false
    );

    public record RagIotIngestRequest(
        string UserId,
        string FarmId,
        string TankId,
        string Ts,
        string? Species = null,
        string? Stage = null,
        double? WaterTemp = null,
        double? PH = null,
        double? AirTemp = null,
        double? PowerWatt = null,
        List<Dictionary<string, object>>? Metrics = null
    );

    public record RagChatResponse(
        string Answer,
        string? AnswerBasis,
        bool NeedsWebSearch,
        string? IntentSource,
        IReadOnlyList<string>? Citations,
        string? Intent = null,
        IReadOnlyList<string>? Intents = null,
        double? Confidence = null,
        IReadOnlyList<string>? Sources = null,
        string? AnswerEngine = null
    );

    public record RagChatFeedbackRequest(
        string UserId,
        string Response,
        bool Helpful,
        string? Intent,
        string? Question
    );

    public record RagChatFeedbackResponse(string Status, bool Saved, string Message);

    public record RagIngestUrlResponse(
        string Status,
        int Documents,
        int Chunks,
        string Title,
        string SourceUrl
    );

    public interface IRagChatClient
    {
        Task<RagChatResponse?> ChatAsync(RagChatRequest request, CancellationToken ct = default);
        Task<RagChatFeedbackResponse?> SubmitFeedbackAsync(RagChatFeedbackRequest request, CancellationToken ct = default);
        Task<RagIngestUrlResponse?> IngestDocumentByUrlAsync(
            string url,
            string title,
            CancellationToken ct = default
        );
    }
}
