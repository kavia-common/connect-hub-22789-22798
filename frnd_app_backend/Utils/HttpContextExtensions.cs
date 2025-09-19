using System.Security.Claims;

namespace FrndAppBackend.Utils
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                      ?? user.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }
}
