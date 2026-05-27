using System.Security.Claims;
using IRasRag.Application.Common.Interfaces.Auth;

namespace IRasRag.API.Utils
{
    public class HttpContextUtils : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUtils(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim == null)
                return null;

            return Guid.TryParse(userIdClaim.Value, out var id) ? id : null;
        }
    }
}
