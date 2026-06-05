using System.Text;
using System.Text.Json;
using IRasRag.Application.Common.Interfaces.Mqtt;
using IRasRag.Application.Common.Models.Mqtt;
using IRasRag.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace IRasRag.Infrastructure.Services.Mqtt
{
    public class MqttPublishService : IMqttPublishService
    {
        private readonly IMqttClient _client;
        private readonly MqttSettings _mqttSettings;
        private readonly ILogger<MqttPublishService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public MqttPublishService(
            IMqttClient client,
            IOptions<MqttSettings> options,
            ILogger<MqttPublishService> logger
        )
        {
            _client = client;
            _mqttSettings = options.Value;
            _logger = logger;
        }

        public async Task PublishCommandAsync(
            string macAddress,
            DeviceCommand command,
            CancellationToken ct = default
        )
        {
            if (!_client.IsConnected)
            {
                _logger.LogWarning(
                    "MQTT client is not connected. Cannot publish command to {MacAddress}.",
                    macAddress
                );
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            var topic = string.Format(_mqttSettings.CommandTopicTemplate, macAddress);
            var payload = JsonSerializer.Serialize(command, JsonOptions);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await _client.PublishAsync(message, ct);

            _logger.LogInformation("Published command to {Topic}: {Payload}", topic, payload);
        }
    }
}
