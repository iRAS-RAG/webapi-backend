using IRasRag.Application.Common.Models.Mqtt;

namespace IRasRag.Application.Common.Interfaces.Mqtt
{
    public interface IMqttPublishService
    {
        Task PublishCommandAsync(
            string macAddress,
            DeviceCommand command,
            CancellationToken ct = default
        );
    }
}
