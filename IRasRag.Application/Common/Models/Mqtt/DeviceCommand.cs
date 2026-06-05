namespace IRasRag.Application.Common.Models.Mqtt
{
    public class DeviceCommand
    {
        public int Pin { get; set; }
        public string Cmd { get; set; } = string.Empty;
    }
}
