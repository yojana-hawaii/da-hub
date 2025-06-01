using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace mvc.Controllers;

public class AccountController : Controller
{
    public IActionResult Login([FromQuery] string returnUrl)
    {
        var redirectUri = returnUrl is null ? Url.Content("~/") : "/" + returnUrl;
        if (!HttpContext.User.Identity.IsAuthenticated)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }
        return LocalRedirect(redirectUri);
    }

    public IActionResult Logout([FromQuery] string returnUrl)
    {
        var redirectUri = returnUrl is null ? Url.Content("~/") : "/" + returnUrl;

        if (!User.Identity.IsAuthenticated)
        {
            return LocalRedirect(redirectUri);
        }

        return new SignOutResult(
            new[]
            {
                OpenIdConnectDefaults.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme
            });
    }
}
