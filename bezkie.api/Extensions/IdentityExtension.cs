using System.Security.Authentication;
using System.Security.Claims;

namespace bezkie.api.Extensions;

public static class IdentityExtension
{
    public static long GetUserId(this IEnumerable<Claim> claims)
    {
        var id = claims.FirstOrDefault(x => x.Type == "UserId").Value;
        if (id == null)
        {
            throw new HttpRequestException(message: "Unauthorized, please try again", statusCode: System.Net.HttpStatusCode.Unauthorized, inner: null);
        }

        return long.Parse(id);
    }
}
