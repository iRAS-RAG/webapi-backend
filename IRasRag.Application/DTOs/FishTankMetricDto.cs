namespace IRasRag.Application.DTOs
{
    public class FishTankMetricDto
    {
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; }
        public IReadOnlyList<SensorMetricDto> SensorMetrics { get; set; }
    }
    public class SensorMetricDto
    {
        public string SensorName { get; set; }
        public string SensorTypeName { get; set; }
        public SensorMetricValueDto Value { get; set; }

    }
    public class SensorMetricValueDto
    {
        public double Value { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Status { get; set; }
    }
}
