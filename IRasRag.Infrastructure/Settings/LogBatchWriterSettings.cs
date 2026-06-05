namespace IRasRag.Infrastructure.Settings
{
    public class LogBatchWriterSettings
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public int MaxRequeueCount { get; set; } = 3;
    }
}
