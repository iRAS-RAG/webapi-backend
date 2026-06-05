namespace IRasRag.Application.DTOs
{
    public class AlertContext
    {
        public Guid AlertId { get; set; }
        public string TankName { get; set; } = default!;
        public Guid TankId { get; set; }
        public Guid FarmId { get; set; }
        public string SensorTypeName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public double TriggerValue { get; set; }
        public double MinThreshold { get; set; }
        public double MaxThreshold { get; set; }
        public string SpeciesName { get; set; } = "unknown";
        public string StageName { get; set; } = "unknown";
    }

    public class RagChatContext
    {
        public string SpeciesName { get; set; } = "unknown";
        public string StageName { get; set; } = "unknown";
    }
}
