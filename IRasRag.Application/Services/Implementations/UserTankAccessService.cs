using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IRasRag.Application.Services.Implementations
{
    public class UserTankAccessService : IUserTankAccessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public UserTankAccessService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<HashSet<Guid>> GetAllowedTankIdsAsync(Guid userId)
        {
            var key = $"user:{userId}:tanks";

            var result =
                await _cache.GetOrCreateAsync(
                    key,
                    async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = CacheTtl;

                        return await UserScopeHelper.GetUserTankIdsAsync(_unitOfWork, userId);
                    }
                )
                ?? throw new InvalidOperationException(
                    "Unexpected null result when retrieving user tank access."
                );

            return new HashSet<Guid>(result);
        }

        public async Task<bool> CanAccessTankAsync(Guid userId, Guid tankId)
        {
            var allowedTanks = await GetAllowedTankIdsAsync(userId);
            return allowedTanks.Contains(tankId);
        }
    }
}
