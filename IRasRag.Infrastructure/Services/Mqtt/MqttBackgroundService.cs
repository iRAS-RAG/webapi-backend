using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Mqtt;
using IRasRag.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using System.Text;
using System.Text.Json;

namespace IRasRag.Infrastructure.Services.Mqtt
{
    public class MqttBackgroundService : BackgroundService, IDisposable
    {
        private readonly ILogger<MqttBackgroundService> _logger;
        private readonly IMqttClient _client;
        private readonly MqttSettings _mqttSettings;
        private readonly IServiceScopeFactory _scopeFactory;
        public MqttBackgroundService(ILogger<MqttBackgroundService> logger, IOptions<MqttSettings> options, IServiceScopeFactory scopeFactory, IMqttClient client)
        {
            _logger = logger;
            _mqttSettings = options.Value;
            _scopeFactory = scopeFactory;
            _client = client;
            _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
            _client.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("MQTT disconnected. Attempting reconnect in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    if (!_client.IsConnected)
                        await ConnectAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reconnect attempt failed.");
                }
            };
        }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            using var scope = _scopeFactory.CreateScope();
            var telemetryDispatchService = scope.ServiceProvider.GetRequiredService<ITelemetryDispatchService>();

            try
            {
                // Validate payload
                if (e.ApplicationMessage?.Payload == null || e.ApplicationMessage.Payload.Length == 0)
                {
                    _logger.LogWarning("Received empty MQTT payload on topic {Topic}",
                        e.ApplicationMessage?.Topic);
                    return;
                }

                // Deserialize  
                var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var dto = JsonSerializer.Deserialize<SensorTelemetry>(json, JsonOptions);

                if (dto == null)
                {
                    _logger.LogWarning("Telemetry payload deserialized to null");
                    return;
                }

                var topic = e.ApplicationMessage.Topic;
                var macFromTopic = topic.Split('/').Last();

                if (dto.Mac != macFromTopic)
                {
                    _logger.LogWarning("MAC mismatch: topic={TopicMac}, payload={PayloadMac}",
                        macFromTopic, dto.Mac);
                }

                await telemetryDispatchService.DispatchAsync(dto, macFromTopic);

                _logger.LogInformation("Successfully processed sensor telemetry from {MacAddress}",
                    macFromTopic);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON in MQTT message: {Payload}",
                    Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process MQTT message from topic {Topic}",
                    e.ApplicationMessage?.Topic);

            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                await ConnectAsync(token);
                // Keep the service alive
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the MQTT service.");
            }
            finally
            {
                await DisconnectAsync();
            }
        }
        private async Task ConnectAsync(CancellationToken token)
        {
            try
            {
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithClientId(_mqttSettings.ClientId)
                    .WithTcpServer(_mqttSettings.Host, _mqttSettings.Port)
                    .WithCredentials(_mqttSettings.Username, _mqttSettings.Password)
                    .WithCleanSession()
                    .Build();

                var response = await _client.ConnectAsync(clientOptions, token);
                var subscribeResult = await _client.SubscribeAsync(_mqttSettings.SubscribeTopic);

                _logger.LogInformation(
                    $"Subscribed to topic {_mqttSettings.SubscribeTopic}. Result: {subscribeResult.Items.FirstOrDefault()?.ResultCode}");


                _logger.LogInformation("The MQTT client is connected.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("MQTT connection cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while connecting to the MQTT broker.");
            }
        }

        private async Task DisconnectAsync()
        {
            if (!_client.IsConnected)
                return;

            try
            {
                _logger.LogInformation("Disconnecting from MQTT broker...");
                await _client.DisconnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while disconnecting MQTT client.");
            }
        }
        public override void Dispose()
        {
            _client?.Dispose();
            base.Dispose();
        }
    }
}
