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
    using MoneySaver.Web.ViewModels.ToDoLists.Enums;

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

        public IActionResult Add()
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

            return this.Redirect("/ToDoLists/All");
        }

        public async Task<IActionResult> DeleteItem(string id, string returnUrl)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.RemoveListItemAsync(user.Id, id);

            if (returnUrl == "/ToDoLists/All")
            {
                return this.RedirectToAction(nameof(this.All));
            }

            return this.Content("No");
        }

        public async Task<IActionResult> All()
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var dtoLists = await this.toDoListsService.GetAll(user.Id);

            var model = new List<ToDoListViewModel>();

            model = dtoLists.Select(l => new ToDoListViewModel
            {
                Id = l.Id,
                Name = l.Name,
                Status = Enum.Parse<StatusType>(l.Status.ToString()),
                CreatedOn = l.CreatedOn,
                ListItems = l.ListItems.Select(li => new ToDoItemViewModel
                {
                    Id = li.Id,
                    Name = li.Name,
                    Status = Enum.Parse<StatusType>(li.Status.ToString()),
                })
                .ToList(),
            })
                .ToList();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeItemStatus(string id, string returnUrl)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (returnUrl == "/ToDoLists/All")
            {
                await this.toDoListsService.ChangeItemStatusAsync(user.Id, id);
                return this.RedirectToAction(nameof(this.All));
            }

            return this.Content("No");
        }
    }
}
