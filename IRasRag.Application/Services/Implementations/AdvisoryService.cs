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
    }
}
