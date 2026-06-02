using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Utils
{
    public static class AlertStatusExtensions
    {
        public static string ToVietnamese(this AlertStatus status) =>
            status switch
            {
                AlertStatus.OPEN => "Chờ xử lý",
                AlertStatus.ACKNOWLEDGED => "Đang xử lý",
                AlertStatus.RESOLVED => "Đóng sự cố",
                AlertStatus.DISMISSED => "Đã bỏ qua",
                _ => status.ToString(),
            };
    }
}
