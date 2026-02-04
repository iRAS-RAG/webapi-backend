namespace IRasRag.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = "IRAS-RAG";
        public string SenderPassword { get; set; } = string.Empty;
    }
}
