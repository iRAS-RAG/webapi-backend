namespace IRasRag.Application.Common.Settings
{
    public class AdvisorySettings
    {
        // Base URLs
        public string ChatBaseUrl { get; set; } = "http://localhost:8002";
        public string IotGatewayBaseUrl { get; set; } = "http://localhost:8001";

        // Chat defaults
        public string DefaultTimeRange { get; set; } = "last_24h";
        public bool AllowWebSearch { get; set; } = false;
        public bool IncludeHistory { get; set; } = false;
        public bool IncludeIntentDebug { get; set; } = false;
        public string? DefaultSpecies { get; set; } = null;
        public string? DefaultStage { get; set; } = null;

        // IoT Gateway endpoints
        public string IotBatchIngestPath { get; set; } = "/iot/ingest/batch";
        public string IotStreamStatsPath { get; set; } = "/iot/stream/stats";
        public string IotStreamPeekPath { get; set; } = "/iot/stream/peek";

        // Chat-RAG endpoints
        public string ChatPath { get; set; } = "/chat";
        public string ChatFeedbackPath { get; set; } = "/chat/feedback";
        public string RagUploadPath { get; set; } = "/rag/ingest-url";

        // Params endpoints
        public string ThresholdsPath { get; set; } = "/params/thresholds";
        public string CatalogPath { get; set; } = "/params/catalog";

        // Telegram notify endpoints
        public string SubscribersPath { get; set; } = "/notify/subscribers";

        // Admin endpoints
        public string AdminRedisHistoryPath { get; set; } = "/admin/redis/history";
        public string AdminRedisKeysUserPath { get; set; } = "/admin/redis/keys/user";
        public string AdminRedisFlushPath { get; set; } = "/admin/redis/flush";
        public string AdminRagDocumentsPath { get; set; } = "/admin/rag/documents";
        public string AdminRagChunksPath { get; set; } = "/admin/rag/chunks";
        public string AdminDataSensorPath { get; set; } = "/admin/data/sensor";
    }

}
