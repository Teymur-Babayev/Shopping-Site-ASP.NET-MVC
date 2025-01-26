using AnadoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnadoolApp.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly LanguageService _localization;

        public ServiceController(LanguageService localization)
        {
            _localization = localization;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyService()
        {
            return View();
        }

        public IActionResult Opportunity()
        {
            return View();
        }
    }
}
