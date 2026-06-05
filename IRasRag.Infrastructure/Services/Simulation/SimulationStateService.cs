using System.Collections.Concurrent;
using IRasRag.Application.Common.Interfaces.Simulation;

namespace IRasRag.Infrastructure.Services.Simulation
{
    public class SimulationStateService : ISimulationStateService
    {
        private readonly ConcurrentDictionary<string, bool> _simulatingMacs = new(
            StringComparer.OrdinalIgnoreCase
        );

        private readonly ConcurrentDictionary<string, DateTime> _lastDispatched = new(
            StringComparer.OrdinalIgnoreCase
        );

        public void StartSimulation(string macAddress)
        {
            _simulatingMacs[macAddress] = true;
            // Reset throttle so the first fake reading fires immediately.
            _lastDispatched.TryRemove(macAddress, out _);
        }

        public void StopSimulation(string macAddress)
        {
            _simulatingMacs.TryRemove(macAddress, out _);
            _lastDispatched.TryRemove(macAddress, out _);
        }

        public bool IsSimulating(string macAddress)
        {
            return _simulatingMacs.ContainsKey(macAddress);
        }

        public IReadOnlySet<string> GetActiveSimulations()
        {
            return new HashSet<string>(_simulatingMacs.Keys, StringComparer.OrdinalIgnoreCase);
        }

        public bool TryAcquireDispatchSlot(string macAddress, TimeSpan minInterval)
        {
            var now = DateTime.UtcNow;
            var acquired = false;

            _lastDispatched.AddOrUpdate(
                macAddress,
                _ =>
                {
                    acquired = true;
                    return now;
                },
                (_, existing) =>
                {
                    if (now - existing >= minInterval)
                    {
                        acquired = true;
                        return now;
                    }

                    acquired = false;
                    return existing;
                }
            );

            return acquired;
        }
    }
}
