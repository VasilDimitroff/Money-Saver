namespace MoneySaver.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Server.HttpSys;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;

    [Area("Administration")]
    public class UsersController : AdministrationController
    {
        private const string IdentifierToMakeUserWithRegularRole = "Regular_User";
        private readonly IUsersService usersService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UsersController(IUsersService usersService, SignInManager<ApplicationUser> signInManager)
        {
            this.usersService = usersService;
            this.signInManager = signInManager;
        }

        // GET: UsersController
        public async Task<IActionResult> Index()
        {
            var model = await this.usersService.GetAllUsersAsync();
            return this.View(model);
        }

        // POST: UsersController/MakeAdmin/5
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var adminRoleId = await this.usersService.GetAdminRoleId();
            await this.usersService.ChangeUserRole(id, adminRoleId);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> MakeUser(string id)
        {
            await this.usersService.ChangeUserRole(id, IdentifierToMakeUserWithRegularRole);

            return this.RedirectToAction(nameof(this.Index));
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private void DeleteCookies()
        {
            foreach (var cookie in this.HttpContext.Request.Cookies)
            {
                this.Response.Cookies.Delete(cookie.Key);
            }
        }
    }
}
