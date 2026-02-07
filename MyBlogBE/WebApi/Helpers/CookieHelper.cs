namespace WebApi.Helpers;

public static class CookieHelper
{
    private static bool _isDevelopment =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    public static void AddCookie(
        HttpResponse response,
        string cookieName,
        string cookieValue,
        int duration
    )
    {
        response.Cookies.Append(
            cookieName,
            cookieValue,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !_isDevelopment,
                SameSite = _isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(duration),
                Path = "/",
            }
        );
    }

    public static void RemoveCookie(HttpResponse response, string cookieName)
    {
        response.Cookies.Delete(
            cookieName
        // ,
        // new CookieOptions
        // {
        //     HttpOnly = true,
        //     Secure = !_isDevelopment,
        //     SameSite = _isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
        //     Path = "/",
        // }
        );
    }
}
