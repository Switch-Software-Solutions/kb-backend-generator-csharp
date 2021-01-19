using System.Linq;
using System.Security.Claims;

namespace Api.Helpers
{
    internal static class AuthorizationHelper
    {
        internal const string ClaimIdentifierUserId = "userId";

        internal static int? GetUserId(this ClaimsPrincipal cp)
        {
            var userIdClaim = cp.Claims.FirstOrDefault(c => c.Type == ClaimIdentifierUserId);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
            return null;
        }

        internal static int? GetUserId(this ClaimsIdentity ci)
        {
            var userIdClaim = ci.Claims.FirstOrDefault(c => c.Type == ClaimIdentifierUserId);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
            return null;
        }
    }
}
