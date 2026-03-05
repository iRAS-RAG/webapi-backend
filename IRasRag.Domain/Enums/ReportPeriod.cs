namespace IRasRag.Domain.Enums
{
    /// <summary>
    /// Các khoảng thời gian hợp lệ cho endpoint dashboard report.
    /// Dùng làm giá trị query parameter: ?period=TODAY | WEEK | MONTH | YEAR
    /// </summary>
    public enum ReportPeriod
    {
        TODAY,
        WEEK,
        MONTH,
        YEAR,
    }
}
