namespace IRasRag.Infrastructure.Settings
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string DocumentFolderPath { get; set; } = "IRAS-RAG/documents";
    }
}
