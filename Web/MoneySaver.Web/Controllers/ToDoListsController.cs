namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.ToDoLists;

    [Authorize]
    public class ToDoListsController : Controller
    {
        private readonly IToDoListsService toDoListsService;
        private readonly UserManager<ApplicationUser> userManager;

        public ToDoListsController(IToDoListsService toDoListsService, UserManager<ApplicationUser> userManager)
        {
            this.toDoListsService = toDoListsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddToDoListInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.AddAsync(user.Id, input.Name, input.ListItems);

            return this.View();
        }
    }
}
