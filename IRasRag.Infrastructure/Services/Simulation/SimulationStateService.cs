using System.Collections.Concurrent;
using IRasRag.Application.Common.Interfaces.Simulation;

namespace IRasRag.Infrastructure.Services.Simulation
{
    public class SimulationStateService : ISimulationStateService
    {
        private readonly ConcurrentDictionary<string, bool> _simulatingMacs = new(
            StringComparer.OrdinalIgnoreCase
        );

        public void StartSimulation(string macAddress)
        {
            _simulatingMacs[macAddress] = true;
        }

        public void StopSimulation(string macAddress)
        {
            _simulatingMacs.TryRemove(macAddress, out _);
        }

        public bool IsSimulating(string macAddress)
        {
            return _simulatingMacs.ContainsKey(macAddress);
        }

        public IReadOnlySet<string> GetActiveSimulations()
        {
            return new HashSet<string>(_simulatingMacs.Keys, StringComparer.OrdinalIgnoreCase);
        }
    }
}
