namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.Shoplists;

    public class ToDoListsController : Controller
    {
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Add(AddToDoListInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            return this.View();
        }
    }
}
