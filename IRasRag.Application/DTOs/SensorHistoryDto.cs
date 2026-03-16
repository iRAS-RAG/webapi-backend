namespace IRasRag.Application.DTOs
{
    public class SensorHistoryDto
    {
        public IReadOnlyList<double> Datasets { get; set; } = new List<double>();
    }
}
