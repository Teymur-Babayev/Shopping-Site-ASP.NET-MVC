using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace AnadoolApp.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var culture = context.RouteData.Values["culture"]?.ToString() ?? "ar";

            // Desteklenen kültürler listesi
            var supportedCultures = new List<string> { "en", "en-US", "tr", "tr-TR", "ar", "ar-SA" };

            // Eğer culture desteklenen kültürler listesinde yoksa, varsayılanı kullan
            if (!supportedCultures.Contains(culture))
            {
                culture = "ar";
            }

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }

    }
}
