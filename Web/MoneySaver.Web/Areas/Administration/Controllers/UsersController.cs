namespace MoneySaver.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Roles;
    using MoneySaver.Web.ViewModels.Users;

    [Area("Administration")]
    public class UsersController : AdministrationController
    {
        private const string IdentifierToMakeUserWithRegularRole = "Regular_User";
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        // GET: UsersController
        public async Task<IActionResult> Index()
        {
            try
            {
                List<UserViewModel> model = await this.GetUsersAsync();

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        // POST: UsersController/MakeAdmin/5
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            try
            {
                var adminRoleId = await this.usersService.GetAdminRoleIdAsync();
                var username = await this.usersService.ChangeUserRoleAsync(id, adminRoleId);
                this.TempData["UserAdmin"] = $"User {username} is now admin!";

                List<UserViewModel> model = await this.GetUsersAsync();

                return this.View("Index", model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MakeUser(string id)
        {
            try
            {
                var username = await this.usersService.ChangeUserRoleAsync(id, IdentifierToMakeUserWithRegularRole);
                this.TempData["UserRegular"] = $"User {username} is now regular user!";

                List<UserViewModel> model = await this.GetUsersAsync();

                return this.View("Index", model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                string username = await this.usersService.UndeleteAsync(id);

                this.TempData["RestoredProfile"] = $"User profile {username} is successfully restored!";

                List<UserViewModel> model = await this.GetUsersAsync();

                return this.View("Index", model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        // GET: UsersController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userDto = await this.usersService.GetUserByIdAsync(id);

                var model = new UserViewModel
                {
                    Id = userDto.Id,
                    CreatedOn = userDto.CreatedOn,
                    Username = userDto.Username,
                    Roles = userDto.Roles.Select(ur => new RoleViewModel
                    {
                        Id = ur.Id,
                        Name = ur.Name,
                    })
                    .ToList(),
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        // POST: UsersController/DeleteConfirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            try
            {
                string username = await this.usersService.MarkAsDeletedAsync(id);
                this.TempData["DeletedProfile"] = $"User profile {username} is marked as deleted!";

                List<UserViewModel> model = await this.GetUsersAsync();

                return this.View("Index", model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        private async Task<List<UserViewModel>> GetUsersAsync()
        {
            var dtoInfo = await this.usersService.GetAllUsersAsync();
            var model = new List<UserViewModel>();

            model = dtoInfo.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                CreatedOn = u.CreatedOn,
                IsDeleted = u.IsDeleted,
                Roles = u.Roles.Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                })
                .ToList(),
            })
                .OrderByDescending(u => u.CreatedOn)
                .ToList();
            return model;
        }
    }
}
