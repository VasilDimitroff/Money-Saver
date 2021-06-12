using Microsoft.AspNetCore.Mvc;

namespace MoneySaver.Web.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult All()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }
    }
}
