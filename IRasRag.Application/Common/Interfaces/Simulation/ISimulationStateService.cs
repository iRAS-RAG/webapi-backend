namespace IRasRag.Application.Common.Interfaces.Simulation
{
    public interface ISimulationStateService
    {
        /// <summary>
        /// Start simulating a "disconnected masterboard" for the given MAC address.
        /// While active, real MQTT data for this MAC will be silently discarded
        /// and fake dangerous readings will be injected instead.
        /// </summary>
        void StartSimulation(string macAddress);

        /// <summary>
        /// Stop the simulation for the given MAC and resume processing real MQTT data.
        /// </summary>
        void StopSimulation(string macAddress);

        /// <summary>
        /// Returns true if the given MAC is currently in simulation mode.
        /// </summary>
        bool IsSimulating(string macAddress);

        /// <summary>
        /// Returns all MAC addresses currently in simulation mode.
        /// </summary>
        IReadOnlySet<string> GetActiveSimulations();
    }
}
