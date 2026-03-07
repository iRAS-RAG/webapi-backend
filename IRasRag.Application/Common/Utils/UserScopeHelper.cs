using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Utils
{
    /// <summary>
    /// Helper chung để giải quyết danh sách FishTankId thuộc phạm vi quyền của một user.
    /// Đường đi: User → UserFarm → Farm → FishTank
    /// Được chia sẻ giữa AnalyticsService và ReportService (và các service khác nếu cần).
    /// </summary>
    public static class UserScopeHelper
    {
        /// <summary>
        /// Trả về tập hợp FishTankId mà user được phép truy cập.
        /// </summary>
        /// <param name="unitOfWork">Unit of work instance.</param>
        /// <param name="userId">Id của user cần kiểm tra quyền.</param>
        /// <param name="farmId">Nếu cung cấp, chỉ lấy tank thuộc farm này (và xác nhận user có quyền truy cập farm đó).</param>
        /// <param name="batchId">Nếu cung cấp, chỉ lấy tank của batch đó (trong phạm vi farm được phép).</param>
        /// <returns>HashSet FishTankId hoặc tập rỗng nếu user không có quyền.</returns>
        public static async Task<HashSet<Guid>> GetUserTankIdsAsync(
            IUnitOfWork unitOfWork,
            Guid userId,
            Guid? farmId = null,
            Guid? batchId = null
        )
        {
            var userFarmRepo = unitOfWork.GetRepository<UserFarm>();
            var userFarmQuery = await userFarmRepo.FindAllAsync(uf => uf.UserId == userId);
            var farmIds = userFarmQuery.Select(uf => uf.FarmId).ToHashSet();

            if (farmId.HasValue)
            {
                if (!farmIds.Contains(farmId.Value))
                    return new HashSet<Guid>();
                farmIds = new HashSet<Guid> { farmId.Value };
            }

            if (farmIds.Count == 0)
                return new HashSet<Guid>();

            var fishTankRepo = unitOfWork.GetRepository<FishTank>();
            var allowedTankIds = (await fishTankRepo.FindAllAsync(t => farmIds.Contains(t.FarmId)))
                .Select(t => t.Id)
                .ToHashSet();

            if (batchId.HasValue)
            {
                var batchRepo = unitOfWork.GetRepository<FarmingBatch>();
                var batch = await batchRepo.FirstOrDefaultAsync(b => b.Id == batchId.Value);
                if (batch != null && allowedTankIds.Contains(batch.FishTankId))
                    return new HashSet<Guid> { batch.FishTankId };

                return new HashSet<Guid>();
            }

            return allowedTankIds;
        }
    }
}
