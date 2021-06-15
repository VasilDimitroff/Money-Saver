using Microsoft.AspNetCore.Mvc;
using MoneySaver.Web.ViewModels.Shoplists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Web.Controllers
{
    public class ShoplistsController : Controller
    {
        public IActionResult Add()
        {
            AddShoplistInputModel model = new AddShoplistInputModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(int id, IEnumerable<string> products)
        {
            AddShoplistInputModel model = new AddShoplistInputModel();

            return Redirect("Wallets/Records/3");
        }
    }
}
