namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Records;
    using MoneySaver.Web.Models.Records.Enums;

    public class RecordsController : Controller
    {
        private readonly IRecordsService recordsService;
        private readonly IWalletsService walletsService;

        public RecordsController(IRecordsService recordsService, IWalletsService walletsService)
        {
            this.recordsService = recordsService;
            this.walletsService = walletsService;
        }

        public async Task<IActionResult> All(int walletId)
        {
            var model = new RecordsWithWalletIdViewModel();

            var records = await this.recordsService.GetRecordsByWalletAsync(walletId);

            model.Records = records.Select(r => new AllRecordsByWalletViewModel
            {
                Amount = r.Amount,
                Category = r.Category,
                CategoryId = r.CategoryId,
                CreatedOn = r.CreatedOn,
                Description = r.Description,
                Wallet = r.Wallet,
                Id = r.Id,
                Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                Currency = r.Currency,
            })
                .ToList();

            model.WalletId = walletId;
            model.Wallet = await this.walletsService.GetWalletNameAsync(walletId);

            return this.View(model);
        }

        public async Task<IActionResult> Search(string searchTerm, int walletId)
        {
            if (searchTerm == null)
            {
                searchTerm = string.Empty;
            }

            var model = new RecordsWithWalletIdViewModel();

            var records = await this.recordsService.GetRecordsByKeywordAsync(searchTerm, walletId);

            model.Records = records.Select(r => new AllRecordsByWalletViewModel
            {
                Amount = r.Amount,
                Category = r.Category,
                CategoryId = r.CategoryId,
                CreatedOn = r.CreatedOn,
                Description = r.Description,
                Wallet = r.Wallet,
                Id = r.Id,
                Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                Currency = r.Currency,
            })
                .ToList();

            model.SearchTerm = searchTerm;
            model.WalletId = walletId;
            model.Wallet = await this.walletsService.GetWalletNameAsync(walletId);

            return this.View("All", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetRecordsByDate(DateTime startDate, DateTime endDate, int walletId)
        {
            var model = new RecordsWithWalletIdViewModel();

            var records = await this.recordsService.GetRecordsByDateRangeAsync(startDate, endDate, walletId);

            model.Records = records.Select(r => new AllRecordsByWalletViewModel
            {
                Amount = r.Amount,
                Category = r.Category,
                CategoryId = r.CategoryId,
                CreatedOn = r.CreatedOn,
                Description = r.Description,
                Wallet = r.Wallet,
                Id = r.Id,
                Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                Currency = r.Currency,
            })
                .ToList();

            model.WalletId = walletId;
            model.Wallet = "Test";

            return this.View(model);
        }

        // GET: RecordsController/Details/5
        public IActionResult Details(int id)
        {
            return this.View();
        }

        public async Task<IActionResult> Add(int walletId)
        {
            AddRecordViewModel model = new AddRecordViewModel();

            var categories = await this.walletsService.GetWalletCategoriesAsync(walletId);

            model.Categories = categories.Select(x => new CategoryNameIdViewModel()
            {
                Name = x.Name,
                Id = x.Id,
                WalletName = x.WalletName,
            });

            model.WalletName = await this.walletsService.GetWalletNameAsync(walletId);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRecordViewModel input)
        {
            var enumValueAsString = input.Type.ToString();
            await this.recordsService.AddAsync(input.CategoryId, input.Description, input.Amount, enumValueAsString);
           //return this.RedirectToAction(nameof(this.All));
            return this.RedirectToAction("All", new { walletId = input.WalletId });
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
                return this.RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RecordsController/Delete/5
        public async Task<IActionResult> Delete(string recordId)
        {
           int wallet = await this.walletsService.GetWalletIdByRecordIdAsync(recordId);
           await this.recordsService.RemoveAsync(recordId);

            // return this.RedirectToAction("All", "Records", new { walletId, action = "Submit", submitAll = false });
           return this.RedirectToAction("All", new { walletId = wallet });
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
