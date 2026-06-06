using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.Advisory.Clients
{
    public class ThresholdSyncClient : IThresholdSyncClient
    {
        private readonly HttpClient _http;
        private readonly AdvisorySettings _settings;
        private readonly ILogger<ThresholdSyncClient> _logger;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public ThresholdSyncClient(
            HttpClient http,
            IOptions<AdvisorySettings> settings,
            ILogger<ThresholdSyncClient> logger
        )
        {
            _http = http;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> CreateAsync(
            string? userId,
            string species,
            string stage,
            string param,
            double min,
            double max,
            CancellationToken ct = default
        )
        {
            var payload = new
            {
                userId,
                farmId = (string?)null,
                species,
                stage,
                param,
                min,
                max,
            };
            var response = await _http.PostAsJsonAsync(
                _settings.ThresholdsPath,
                payload,
                SerializerOptions,
                ct
            );

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Advisory POST {_settings.ThresholdsPath} returned {(int)response.StatusCode} for {species}/{stage}/{param}"
                );

            var dto = await response.Content.ReadFromJsonAsync<ThresholdResponseDto>(
                SerializerOptions,
                ct
            );
            if (dto?.Id == null)
                throw new InvalidOperationException(
                    $"Advisory POST {_settings.ThresholdsPath} returned no ID for {species}/{stage}/{param}"
                );

            return dto.Id.Value.ToString();
        }

        public async Task UpdateAsync(
            string advisoryId,
            double min,
            double max,
            CancellationToken ct = default
        )
        {
            var payload = new { min, max };
            var response = await _http.PutAsJsonAsync(
                $"{_settings.ThresholdsPath}/{advisoryId}",
                payload,
                SerializerOptions,
                ct
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Advisory PUT {Path}/{AdvisoryId} returned 404 â€” record absent in advisory app, skipping update",
                    _settings.ThresholdsPath,
                    advisoryId
                );
                return;
            }

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Advisory PUT {_settings.ThresholdsPath}/{advisoryId} returned {(int)response.StatusCode}"
                );
        }

        public async Task DeleteAsync(string advisoryId, CancellationToken ct = default)
        {
            var response = await _http.DeleteAsync($"{_settings.ThresholdsPath}/{advisoryId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Advisory DELETE {Path}/{AdvisoryId} returned 404 â€” already absent, treating as success",
                    _settings.ThresholdsPath,
                    advisoryId
                );
                return;
            }

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Advisory DELETE {_settings.ThresholdsPath}/{advisoryId} returned {(int)response.StatusCode}"
                );
        }

        private sealed class ThresholdResponseDto
        {
            [JsonPropertyName("id")]
            public int? Id { get; set; }
        }
    }
}

