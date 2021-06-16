using Microsoft.AspNetCore.Mvc;
using MoneySaver.Web.ViewModels.Shoplists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Web.Controllers
{
    public class ToDoListsController : Controller
    {
        public IActionResult Add()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddToDoListInputModel input)
        {
            if (ModelState.IsValid)
            {

            }

            return Json(ModelState);
        }
    }
}
