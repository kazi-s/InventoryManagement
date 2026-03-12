using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Models.Entities;
using Microsoft.AspNetCore.Localization;

namespace InventoryManagement.Controllers
{
    public abstract class BaseController : Controller
    {
        private UserManager<User>? _userManager;
        
        protected UserManager<User> UserManager => 
            _userManager ??= HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

        protected string? GetUserId() => UserManager.GetUserId(User);

        protected void SetTheme(string theme)
        {
            HttpContext.Session.SetString("Theme", theme);
        }

        protected string GetTheme()
        {
            return HttpContext.Session.GetString("Theme") ?? "light";
        }

        protected void SetLanguage(string culture)
        {
            HttpContext.Session.SetString("Language", culture);
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        protected string GetLanguage()
        {
            return HttpContext.Session.GetString("Language") ?? "en";
        }
    }
}