using System.Security.Claims;

namespace FlowSync.Auth
{
    public static class IdentityExtensions
    {
        public static Guid? GetUserId(this HttpContext context) {
            var userId = context.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId?.Value, out var parsedId)) {
                return parsedId;
            }

            return null;
        }
    }
}
