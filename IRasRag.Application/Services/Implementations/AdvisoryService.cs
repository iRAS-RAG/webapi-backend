using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Settings;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.AlertSpecifications;
using IRasRag.Application.Specifications.FishTankSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IRasRag.Application.Services.Implementations
{
    public class AdvisoryService : IAdvisoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRagChatClient _ragChatClient;
        private readonly AdvisorySettings _settings;
        private readonly ILogger<AdvisoryService> _logger;

        public AdvisoryService(
            IUnitOfWork unitOfWork,
            IRagChatClient ragChatClient,
            IOptions<AdvisorySettings> settings,
            ILogger<AdvisoryService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _ragChatClient = ragChatClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task GenerateForAlertAsync(Guid alertId)
        {
            try
            {
                var context = await _unitOfWork
                    .GetRepository<Alert>()
                    .FirstOrDefaultAsync(new AlertContextSpec(alertId));

                if (context == null)
                {
                    _logger.LogWarning(
                        "Advisory: alert context not found for alert {AlertId}",
                        alertId
                    );

                    return;
                }

                var message = BuildRecommendationMessage(context);

                var chatRequest = new RagChatRequest(
                    UserId: context.AlertId.ToString(),
                    FarmId: context.FarmId.ToString(),
                    TankId: context.TankId.ToString(),
                    Species: context.SpeciesName,
                    Stage: context.StageName,
                    Message: message,
                    TimeRange: _settings.DefaultTimeRange,
                    AllowWebSearch: _settings.AllowWebSearch
                );

                var response = await _ragChatClient.ChatAsync(chatRequest);

                if (response == null || string.IsNullOrWhiteSpace(response.Answer))
                {
                    _logger.LogWarning(
                        "Advisory: empty answer from RAG for alert {AlertId}",
                        alertId
                    );

                    return;
                }

                var recommendation = new Recommendation
                {
                    Id = Guid.NewGuid(),
                    AlertId = alertId,
                    DocumentId = null,
                    SuggestionText = response.Answer.Trim(),
                };

                await _unitOfWork.GetRepository<Recommendation>().AddAsync(recommendation);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Advisory: recommendation {RecId} saved for alert {AlertId} (basis={Basis})",
                    recommendation.Id,
                    alertId,
                    response.AnswerBasis
                );
            }
            catch (Exception ex)
            {
                // advisory must never break alert pipeline
                _logger.LogError(
                    ex,
                    "Advisory: unhandled error generating recommendation for alert {AlertId}",
                    alertId
                );
            }
        }

        public async Task<AdvisoryChatResult> ChatAsync(
            Guid tankId,
            Guid userId,
            string message,
            CancellationToken ct = default
        )
        {
            var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(tankId);

            if (tank == null)
            {
                _logger.LogWarning(
                    "Advisory chat: tank {TankId} not found for user {UserId}",
                    tankId,
                    userId
                );

                return new AdvisoryChatResult("Không tìm thấy bể nuôi.", IsOffTopic: false);
            }

            var context = await _unitOfWork
                .GetRepository<FishTank>()
                .FirstOrDefaultAsync(new RagChatContextSpec(tankId));

            var chatRequest = new RagChatRequest(
                UserId: userId.ToString(),
                FarmId: tank.FarmId.ToString(),
                TankId: tankId.ToString(),
                Species: context?.SpeciesName ?? "unknown",
                Stage: context?.StageName ?? "unknown",
                Message: message,
                TimeRange: _settings.DefaultTimeRange,
                AllowWebSearch: _settings.AllowWebSearch
            );

            var response = await _ragChatClient.ChatAsync(chatRequest, ct);

            if (response == null || string.IsNullOrWhiteSpace(response.Answer))
            {
                _logger.LogWarning(
                    "Advisory chat: empty answer from RAG for user {UserId}, tank {TankId}",
                    userId,
                    tankId
                );

                return new AdvisoryChatResult(
                    "Không thể lấy câu trả lời từ hệ thống tư vấn. Vui lòng thử lại.",
                    IsOffTopic: false
                );
            }

            var intent = response.Intent ?? response.IntentSource;
            var isOffTopic = intent == "off_topic";

            _logger.LogInformation(
                "Advisory chat: intent={Intent} basis={Basis} for user {UserId}, tank {TankId}",
                intent,
                response.AnswerBasis,
                userId,
                tankId
            );

            return new AdvisoryChatResult(
                response.Answer.Trim(),
                IsOffTopic: isOffTopic,
                Citations: response.Citations,
                Intent: intent
            );
        }

        public async Task<MortalityDiagnosisResult> DiagnoseMortalityAsync(
            Guid tankId,
            Guid userId,
            Guid? batchId = null,
            string? timeRange = null,
            string? message = null,
            CancellationToken ct = default
        )
        {
            // 1. Get tank info
            var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(tankId);
            if (tank == null)
            {
                _logger.LogWarning(
                    "Mortality diagnosis: tank {TankId} not found for user {UserId}",
                    tankId,
                    userId
                );
                return new MortalityDiagnosisResult(
                    "Không tìm thấy bể nuôi.",
                    "error",
                    null,
                    null,
                    "error"
                );
            }

            // 2. Determine time window
            var effectiveTimeRange = timeRange ?? "last_7d";
            var now = DateTime.UtcNow;
            var windowStart = effectiveTimeRange.ToLowerInvariant() switch
            {
                "last_24h" or "24h" => now.AddHours(-24),
                "last_48h" or "48h" => now.AddHours(-48),
                "last_3d" or "3d" => now.AddDays(-3),
                "last_7d" or "7d" or "week" => now.AddDays(-7),
                "last_14d" or "14d" => now.AddDays(-14),
                "last_30d" or "30d" or "month" => now.AddDays(-30),
                _ => now.AddDays(-7),
            };

            // 3. Determine effective batch(es) to query
            Guid? effectiveBatchId = batchId;
            string? speciesName = null;
            string? stageName = null;

            if (effectiveBatchId.HasValue)
            {
                var batch = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .GetByIdAsync(effectiveBatchId.Value);
                if (batch?.CurrentStageConfig != null)
                {
                    speciesName = batch.CurrentStageConfig.Species?.Name;
                    stageName = batch.CurrentStageConfig.GrowthStage?.Name;
                }
            }
            else
            {
                // Find active batch for this tank
                var activeBatch = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .FirstOrDefaultAsync(fb =>
                        fb.FishTankId == tankId && fb.Status == FarmingBatchStatus.ACTIVE
                    );
                effectiveBatchId = activeBatch?.Id;
                if (activeBatch?.CurrentStageConfig != null)
                {
                    speciesName = activeBatch.CurrentStageConfig.Species?.Name;
                    stageName = activeBatch.CurrentStageConfig.GrowthStage?.Name;
                }
            }

            // If still no species/stage, try the RagChatContextSpec
            if (string.IsNullOrWhiteSpace(speciesName))
            {
                var context = await _unitOfWork
                    .GetRepository<FishTank>()
                    .FirstOrDefaultAsync(new RagChatContextSpec(tankId));
                speciesName = context?.SpeciesName ?? "unknown";
                stageName = context?.StageName ?? "unknown";
            }

            // 4. Query mortality logs in the time window
            var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
            IReadOnlyList<MortalityLog> mortalityLogs;
            if (effectiveBatchId.HasValue)
            {
                mortalityLogs = await mortalityRepo.FindAllAsync(ml =>
                    ml.BatchId == effectiveBatchId.Value && ml.Date >= windowStart
                );
            }
            else
            {
                // No batch — query all mortality for batches in this tank
                mortalityLogs = await mortalityRepo.FindAllAsync(ml =>
                    ml.Batch.FishTankId == tankId && ml.Date >= windowStart
                );
            }

            // 5. Query feeding logs in the time window
            var feedingRepo = _unitOfWork.GetRepository<FeedingLog>();
            IReadOnlyList<FeedingLog> feedingLogs;
            if (effectiveBatchId.HasValue)
            {
                feedingLogs = await feedingRepo.FindAllAsync(fl =>
                    fl.FarmingBatchId == effectiveBatchId.Value && fl.CreatedDate >= windowStart
                );
            }
            else
            {
                feedingLogs = await feedingRepo.FindAllAsync(fl =>
                    fl.FarmingBatch.FishTankId == tankId && fl.CreatedDate >= windowStart
                );
            }

            // 6. Query alerts in the time window
            var alertRepo = _unitOfWork.GetRepository<Alert>();
            IReadOnlyList<Alert> alerts;
            if (effectiveBatchId.HasValue)
            {
                alerts = await alertRepo.FindAllAsync(a =>
                    a.FishTankId == tankId && a.RaisedAt >= windowStart
                );
            }
            else
            {
                alerts = await alertRepo.FindAllAsync(a =>
                    a.FishTankId == tankId && a.RaisedAt >= windowStart
                );
            }

            // 7. Build structured context for the AI RAG
            var mortalityContext = BuildMortalityContextDict(mortalityLogs, effectiveTimeRange);
            var feedingContext = BuildFeedingContextDict(feedingLogs, effectiveTimeRange);
            var alertContext = BuildAlertContextDict(alerts);

            // 8. Build diagnostic message if not provided
            var diagnosticMessage =
                message
                ?? BuildMortalityDiagnosticMessage(
                    mortalityLogs,
                    feedingLogs,
                    alerts,
                    speciesName,
                    stageName,
                    effectiveTimeRange
                );

            // 9. Send to AI RAG with all context
            var chatRequest = new RagChatRequest(
                UserId: userId.ToString(),
                FarmId: tank.FarmId.ToString(),
                TankId: tankId.ToString(),
                Species: speciesName ?? "unknown",
                Stage: stageName ?? "unknown",
                Message: diagnosticMessage,
                TimeRange: effectiveTimeRange,
                AllowWebSearch: _settings.AllowWebSearch,
                MortalityContext: mortalityContext,
                FeedingContext: feedingContext,
                AlertContext: alertContext
            );

            var response = await _ragChatClient.ChatAsync(chatRequest, ct);

            if (response == null || string.IsNullOrWhiteSpace(response.Answer))
            {
                _logger.LogWarning(
                    "Mortality diagnosis: empty answer from AI for user {UserId}, tank {TankId}",
                    userId,
                    tankId
                );
                return new MortalityDiagnosisResult(
                    "Không thể lấy chẩn đoán từ hệ thống AI. Vui lòng thử lại.",
                    "error",
                    null,
                    null,
                    "error"
                );
            }

            var intent = response.Intent ?? response.IntentSource ?? "mortality_diagnosis";

            _logger.LogInformation(
                "Mortality diagnosis: intent={Intent} basis={Basis} for user {UserId}, tank {TankId}",
                intent,
                response.AnswerBasis,
                userId,
                tankId
            );

            return new MortalityDiagnosisResult(
                Answer: response.Answer.Trim(),
                Intent: intent,
                Confidence: response.Confidence,
                Citations: response.Citations,
                AnswerBasis: response.AnswerBasis ?? "iot+rag+mortality_context"
            );
        }

        public async Task<RagChatFeedbackResponse?> SubmitFeedbackAsync(
            Guid userId,
            string response,
            bool helpful,
            string? intent,
            string? question,
            CancellationToken ct = default
        )
        {
            var request = new RagChatFeedbackRequest(
                UserId: userId.ToString(),
                Response: response,
                Helpful: helpful,
                Intent: intent,
                Question: question
            );

            return await _ragChatClient.SubmitFeedbackAsync(request, ct);
        }

        private static string BuildRecommendationMessage(AlertContext context)
        {
            var unit = string.IsNullOrWhiteSpace(context.Unit) ? "" : $" {context.Unit}";

            var message =
                $"{context.SensorTypeName} tại bể {context.TankName} "
                + $"đo được {context.TriggerValue}{unit} "
                + $"(ngưỡng: {context.MinThreshold}–{context.MaxThreshold}{unit}). ";

            var hasSpecies = !string.Equals(
                context.SpeciesName,
                "unknown",
                StringComparison.OrdinalIgnoreCase
            );

            var hasStage = !string.Equals(
                context.StageName,
                "unknown",
                StringComparison.OrdinalIgnoreCase
            );

            if (hasSpecies && hasStage)
            {
                message +=
                    $"Đối tượng nuôi: {context.SpeciesName}, giai đoạn {context.StageName}. "
                    + $"Theo SOP cần xử lý như thế nào ? Đưa ra đề xuất ngắn gọn và phù hợp.";
            }
            else
            {
                message +=
                    "Hiện không có vụ nuôi đang hoạt động trong bể. "
                    + "Hãy đánh giá mức độ bất thường của chỉ số nước và đề xuất hành động phù hợp.";
            }

            return message;
        }

        // ── Mortality diagnosis helpers ──

        private static Dictionary<string, object> BuildMortalityContextDict(
            IReadOnlyList<MortalityLog> logs,
            string timeRange
        )
        {
            var ctx = new Dictionary<string, object>
            {
                ["totalDeaths"] = logs.Sum(ml => ml.Quantity),
                ["period"] = timeRange,
                ["events"] = logs.OrderByDescending(ml => ml.Date)
                    .Take(30)
                    .Select(ml => new Dictionary<string, object>
                    {
                        ["date"] = ml.Date.ToString("dd/MM/yyyy"),
                        ["quantity"] = ml.Quantity,
                        ["lostWeightKg"] = Math.Round(ml.LostWeightKg, 2),
                    })
                    .ToList<object>(),
            };
            return ctx;
        }

        private static Dictionary<string, object> BuildFeedingContextDict(
            IReadOnlyList<FeedingLog> logs,
            string timeRange
        )
        {
            var ctx = new Dictionary<string, object>
            {
                ["totalAmount"] = Math.Round(logs.Sum(fl => fl.Amount), 2),
                ["period"] = timeRange,
                ["logs"] = logs.OrderByDescending(fl => fl.CreatedDate)
                    .Take(30)
                    .Select(fl => new Dictionary<string, object>
                    {
                        ["date"] = fl.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                        ["amount"] = Math.Round(fl.Amount, 2),
                        ["feedType"] = fl.FeedType?.Name ?? "unknown",
                    })
                    .ToList<object>(),
            };
            return ctx;
        }

        private static Dictionary<string, object> BuildAlertContextDict(IReadOnlyList<Alert> alerts)
        {
            var ctx = new Dictionary<string, object>
            {
                ["alerts"] = alerts
                    .OrderByDescending(a => a.RaisedAt)
                    .Take(30)
                    .Select(a => new Dictionary<string, object>
                    {
                        ["sensorType"] = a.SensorType?.Name ?? "unknown",
                        ["value"] = a.TriggerValue,
                        ["threshold"] =
                            $"{a.SpeciesThreshold?.MinValue}–{a.SpeciesThreshold?.MaxValue} {a.SensorType?.UnitOfMeasure ?? ""}".Trim(),
                        ["raisedAt"] = a.RaisedAt.ToString("dd/MM/yyyy HH:mm"),
                        ["status"] = a.Status.ToString(),
                    })
                    .ToList<object>(),
            };
            return ctx;
        }

        private static string BuildMortalityDiagnosticMessage(
            IReadOnlyList<MortalityLog> mortalityLogs,
            IReadOnlyList<FeedingLog> feedingLogs,
            IReadOnlyList<Alert> alerts,
            string? species,
            string? stage,
            string timeRange
        )
        {
            var totalDead = mortalityLogs.Sum(ml => ml.Quantity);
            var totalFeed = Math.Round(feedingLogs.Sum(fl => fl.Amount), 2);
            var alertCount = alerts.Count;

            var speciesLine =
                string.IsNullOrWhiteSpace(species) || species == "unknown"
                    ? ""
                    : $"Đối tượng nuôi: {species}"
                        + (
                            string.IsNullOrWhiteSpace(stage) || stage == "unknown"
                                ? ""
                                : $", giai đoạn: {stage}"
                        );

            var message =
                "Phân tích nguyên nhân cá chết:\n"
                + $"Trong khoảng thời gian {timeRange}, ghi nhận tổng cộng {totalDead} con cá chết";

            if (totalFeed > 0)
                message += $", tổng lượng thức ăn đã cho là {totalFeed} kg";
            if (alertCount > 0)
                message += $", có {alertCount} cảnh báo được kích hoạt";

            message += ".";

            if (!string.IsNullOrWhiteSpace(speciesLine))
                message += $"\n{speciesLine}.";

            message +=
                "\n\nHãy phân tích TOÀN DIỆN các dữ liệu cảm biến, lịch sử cho ăn, "
                + "lịch sử cá chết và cảnh báo được cung cấp bên dưới để xác định "
                + "nguyên nhân cá chết (chất lượng nước, cho ăn quá mức, bệnh, sốc môi trường...), "
                + "xếp hạng theo mức độ khả năng và đưa ra hành động khắc phục cụ thể.";

            return message;
        }
    }
}
