namespace MoneySaver.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;


    public class RecordsController : Controller
    {
        private readonly IRecordsService recordsService;

        public RecordsController(IRecordsService recordsService)
        {
            this.recordsService = recordsService;
        }

        public IActionResult All()
        {
            return View();
        }

        // GET: RecordsController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: RecordsController/Create
        public IActionResult Add()
        {
            return View();
        }

        // POST: RecordsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(RecordInputModel input)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
                this.recordsService.AddAsync("078-203d-34", input.CategoryId, input.WalletId, input.Description, input.Amount, input.Type);
                return Redirect("/");
            }
            catch
            {
                return Redirect("/");
            }
        }

        // GET: RecordsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RecordsController/Edit/5
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

        // GET: RecordsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RecordsController/Delete/5
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
    }
}
