using AnadoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnadoolApp.Controllers
{
    public class BlogController : BaseController
    {
        private readonly LanguageService _localization;

        public BlogController(LanguageService localization)
        {
            _localization = localization;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyBlog()
        {
            return View();
        }
    }
}
