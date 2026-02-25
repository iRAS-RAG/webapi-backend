using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Utils
{
    public static class BatchPausedReasonExtension
    {
        public static bool AllowsAddNewBatch(this BatchPausedReason reason)
        {
            return reason switch
            {
                BatchPausedReason.GRADING => true,
                BatchPausedReason.TANK_CLEANING => true,
                BatchPausedReason.SENSOR_CALIBRATION => false,
                _ => false,
            };
        }
    }
}
