using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.Advisory
{
    public class CatalogSyncClient : ICatalogSyncClient
    {
        private readonly HttpClient _http;
        private readonly AdvisorySettings _settings;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public CatalogSyncClient(HttpClient http, IOptions<AdvisorySettings> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task UpsertAsync(
            string code,
            string? labelVi,
            string? unit,
            CancellationToken ct = default
        )
        {
            var payload = new CatalogUpsertPayload(code, labelVi, unit);
            var response = await _http.PostAsJsonAsync(
                _settings.CatalogPath,
                payload,
                SerializerOptions,
                ct
            );

            if (!response.IsSuccessStatusCode)
            {
                var detail = await TryReadDetailAsync(response, ct);
                var suffix = detail is not null ? $": {detail}" : string.Empty;
                throw new HttpRequestException(
                    $"Advisory POST {_settings.CatalogPath} returned {(int)response.StatusCode} for code '{code}'{suffix}"
                );
            }
        }

        public async Task DeleteAsync(string code, CancellationToken ct = default)
        {
            var response = await _http.DeleteAsync($"{_settings.CatalogPath}/{code}", ct);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Advisory DELETE {_settings.CatalogPath}/{code} returned {(int)response.StatusCode}"
                );
        }

        private static async Task<string?> TryReadDetailAsync(
            HttpResponseMessage response,
            CancellationToken ct
        )
        {
            try
            {
                var body = await response.Content.ReadFromJsonAsync<ErrorBody>(
                    SerializerOptions,
                    ct
                );
                return body?.Detail;
            }
            catch
            {
                return null;
            }
        }

        private sealed record CatalogUpsertPayload(
            [property: JsonPropertyName("code")] string Code,
            [property: JsonPropertyName("label_vi")] string? LabelVi,
            [property: JsonPropertyName("unit")] string? Unit
        );

        private sealed record ErrorBody([property: JsonPropertyName("detail")] string? Detail);
    }
}
