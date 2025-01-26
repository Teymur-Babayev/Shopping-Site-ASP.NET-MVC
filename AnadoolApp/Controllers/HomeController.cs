using AnadoolApp.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AnadoolApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly LanguageService _localization;

        public HomeController(LanguageService localization)
        {
            _localization = localization;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            ViewBag.AboutMe = _localization.GetTextFromFile("AboutMe", cultureInfo);
            ViewBag.MyPrinciples = _localization.GetTextFromFile("MyPrinciples", cultureInfo);

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            var cultureInfo = CultureInfo.CurrentCulture; // Kullanıcının mevcut kültür bilgisi
            ViewBag.PrivacyPolicyText = _localization.GetTextFromFile("PrivacyPolicyText", cultureInfo);
            return View();
        }

        #region Localization
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                // returnUrl içindeki mevcut dil kodunu yeni seçilen dil kodu ile değiştir
                var uri = new Uri(string.Concat($"{Request.Scheme}://{Request.Host}", returnUrl));

                // returnUrl'den query string'i ve scheme://host kısmını çıkararak saf path'i al
                var path = uri.PathAndQuery;

                // Mevcut dil kodunu yeni seçilen dil kodu ile değiştir
                var newPath = Regex.Replace(path, "^/(ar|en|tr)", $"/{culture}", RegexOptions.IgnoreCase);

                // Tam URL'yi oluştur
                var newUrl = $"{Request.Scheme}://{Request.Host}{newPath}";

                return Redirect(newUrl);
            }

            // Geçerli bir returnUrl yoksa veya dış URL ise, kullanıcıyı yeni dildeki ana sayfaya yönlendir
            return RedirectToAction("Index", "Home", new { culture = culture });
        }
        #endregion

        public IActionResult StatusCode(int code)
        {
            if (code == 404)
            {
                return View("NotFound");
            }
            return View("Error");
        }

    }
}
