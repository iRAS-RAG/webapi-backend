namespace IRasRag.Infrastructure.Settings
{
    public class MqttSettings
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SubscribeTopic { get; set; }
        public string CommandTopicTemplate { get; set; }
        public bool UseTls { get; set; } = false;

        public void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException("Mqtt Name is missing or empty");
            if (string.IsNullOrWhiteSpace(Host))
                throw new InvalidOperationException("Mqtt Host is missing or empty");
            if (Port <= 0)
                throw new InvalidOperationException("Mqtt Port must be greater than 0");
            if (string.IsNullOrWhiteSpace(ClientId))
                throw new InvalidOperationException("Mqtt ClientId is missing or empty");
            if (string.IsNullOrWhiteSpace(Username))
                throw new InvalidOperationException("Mqtt Username is missing or empty");
            if (string.IsNullOrWhiteSpace(Password))
                throw new InvalidOperationException("Mqtt Password is missing or empty");
        }
    }
}
