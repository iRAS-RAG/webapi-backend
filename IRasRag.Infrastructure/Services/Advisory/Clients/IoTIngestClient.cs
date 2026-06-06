using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Models.Advisory;
using IRasRag.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.Advisory.Clients
{
    public class IoTIngestClient : IIoTIngestClient
    {
        private readonly HttpClient _http;
        private readonly AdvisorySettings _settings;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public IoTIngestClient(HttpClient http, IOptions<AdvisorySettings> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task IngestBatchAsync(
            IoTBatchIngestPayload payload,
            CancellationToken ct = default
        )
        {
            var response = await _http.PostAsJsonAsync(
                _settings.IotBatchIngestPath,
                payload,
                SerializerOptions,
                ct
            );

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Advisory IoT batch ingest POST {_settings.IotBatchIngestPath} returned {(int)response.StatusCode} "
                        + $"for {payload.Events.Count} events"
                );
        }

        private sealed class IngestResponseDto
        {
            [JsonPropertyName("status")]
            public string? Status { get; set; }

            [JsonPropertyName("queued")]
            public bool Queued { get; set; }

            [JsonPropertyName("entryId")]
            public string? EntryId { get; set; }
        }
    }
}
