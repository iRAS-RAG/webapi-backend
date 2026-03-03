using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// Compares multiple FarmingBatches across a set of metrics (F11 – Supervisor).
        /// </summary>
        Task<Result<BatchCompareResponseDto>> CompareBatchesAsync(BatchCompareRequest request);

        /// <summary>
        /// Returns alert frequency statistics by sensor type (F12 – Supervisor).
        /// </summary>
        Task<Result<AlertFrequencyResponseDto>> GetAlertFrequencyAsync(AlertFrequencyRequest request);
    }
}
