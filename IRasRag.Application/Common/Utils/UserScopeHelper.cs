using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Specifications.FishTankSpecifications;
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
            var allowedTankIds = await unitOfWork
                .GetRepository<FishTank>()
                .ListAsync(new UserAllowedFishTankSpec(userId, farmId, batchId));
            return allowedTankIds.ToHashSet();
        }
    }
}
