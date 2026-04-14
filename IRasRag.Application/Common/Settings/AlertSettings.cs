namespace IRasRag.Application.Common.Settings
{
    public class AlertSettings
    {
        /// <summary>
        /// Number of consecutive breaches required to trigger an alert. For example, if set to 3, a temperature alert would only trigger after 3 consecutive readings above the threshold, helping to filter out transient spikes that might otherwise cause false alarms.
        /// </summary>
        public int BreachConfirmationCount { get; set; } = 3;

        /// <summary>
        /// Number of consecutive breaches required to resolve an active alert. For example, if set to 5, an active temperature alert would only resolve after 5 consecutive readings below the threshold, providing a buffer against transient dips that might otherwise cause alert flapping.
        /// </summary>
        public int ResolveCount { get; set; } = 5;

        /// <summary>
        /// Percentage threshold for hysteresis to prevent alert flapping. For example, if set to 0.05, a temperature alert that triggers at 30°C would only count for resolve once the temperature drops below 28.5°C (5% below the threshold).
        /// </summary>
        public double HysteresisPercentage { get; set; } = 0.02;
    }
}
