using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.Advisory
{
    public class RagChatClient : IRagChatClient
    {
        private readonly HttpClient _http;
        private readonly AdvisorySettings _settings;
        private readonly ILogger<RagChatClient> _logger;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public RagChatClient(HttpClient http, IOptions<AdvisorySettings> settings, ILogger<RagChatClient> logger)
        {
            _http = http;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<RagChatResponse?> ChatAsync(RagChatRequest request, CancellationToken ct = default)
        {
            try
            {
                var payload = new
                {
                    userId = request.UserId,
                    farmId = request.FarmId,
                    tankId = request.TankId,
                    species = request.Species,
                    stage = request.Stage,
                    message = request.Message,
                    timeRange = request.TimeRange,
                    allowWebSearch = request.AllowWebSearch,
                };

                var httpResponse = await _http.PostAsJsonAsync(_settings.ChatPath, payload, SerializerOptions, ct);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "RAG /chat returned {Status} for userId={UserId}",
                        (int)httpResponse.StatusCode, request.UserId);
                    return null;
                }

                var dto = await httpResponse.Content.ReadFromJsonAsync<RagChatResponseDto>(ct);
                if (dto == null) return null;

                return new RagChatResponse(
                    Answer: dto.Answer ?? string.Empty,
                    AnswerBasis: dto.AnswerBasis,
                    NeedsWebSearch: dto.NeedsWebSearch,
                    IntentSource: dto.IntentSource,
                    Citations: dto.Citations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RAG /chat call failed for userId={UserId}", request.UserId);
                return null;
            }
        }

        public async Task<RagIngestUrlResponse?> IngestDocumentByUrlAsync(string url, string title, CancellationToken ct = default)
        {
            try
            {
                var payload = new { url, title };
                var httpResponse = await _http.PostAsJsonAsync(_settings.RagUploadPath, payload, SerializerOptions, ct);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("RAG /rag/ingest-url returned {Status} for url={Url}", (int)httpResponse.StatusCode, url);
                    return null;
                }

                var dto = await httpResponse.Content.ReadFromJsonAsync<RagIngestUrlResponseDto>(ct);
                if (dto == null) return null;

                return new RagIngestUrlResponse(
                    Status: dto.Status ?? string.Empty,
                    Documents: dto.Documents,
                    Chunks: dto.Chunks,
                    Title: dto.Title ?? string.Empty,
                    SourceUrl: dto.SourceUrl ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RAG /rag/ingest-url failed for url={Url}", url);
                return null;
            }
        }

        private sealed class RagIngestUrlResponseDto
        {
            [JsonPropertyName("status")]
            public string? Status { get; set; }

            [JsonPropertyName("documents")]
            public int Documents { get; set; }

            [JsonPropertyName("chunks")]
            public int Chunks { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("source_url")]
            public string? SourceUrl { get; set; }
        }

        private sealed class RagChatResponseDto
        {
            [JsonPropertyName("answer")]
            public string? Answer { get; set; }

            [JsonPropertyName("answer_basis")]
            public string? AnswerBasis { get; set; }

            [JsonPropertyName("needs_web_search")]
            public bool NeedsWebSearch { get; set; }

            [JsonPropertyName("intent_source")]
            public string? IntentSource { get; set; }

            [JsonPropertyName("citations")]
            public List<string>? Citations { get; set; }
        }
    }
}
