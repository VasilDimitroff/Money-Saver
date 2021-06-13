namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Globalization;
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
                CreatedOn = r.CreatedOn.ToString("D", CultureInfo.InvariantCulture),
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
                CreatedOn = r.CreatedOn.ToString("D", CultureInfo.InvariantCulture),
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
                CreatedOn = r.CreatedOn.ToString("D", CultureInfo.InvariantCulture),
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
           // return this.RedirectToAction(nameof(this.All));
            return this.RedirectToAction("All", new { walletId = input.WalletId });
        }

        // GET: RecordsController/Edit/5
        public async Task<IActionResult> Edit(string id, int walletId)
        {
            if (id == null)
            {
                this.RedirectToAction("All", new { walletId = walletId });
            }

            var recordDto = await this.recordsService.GetRecordByIdAsync(id, walletId);

            EditRecordViewModel model = new EditRecordViewModel()
            {
                Id = recordDto.Id,
                Description = recordDto.Description,
                ModifiedOn = recordDto.ModifiedOn.HasValue ? recordDto.ModifiedOn.Value : null,
                Type = Enum.Parse<RecordTypeInputModel>(recordDto.Type.ToString()),
                WalletId = walletId,
                Amount = recordDto.Amount,
                WalletName = recordDto.WalletName,
                CategoryId = recordDto.CategoryId,
                Categories = recordDto.Categories.Select(c => new CategoryNameIdViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    WalletName = c.WalletName,
                })
                .ToList(),
            };
            return this.View(model);
        }

        // POST: RecordsController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(EditRecordInputModel input)
        {
            await this.recordsService.UpdateRecord(input.Id, input.CategoryId, input.WalletId, input.Description, input.Amount, input.Type, input.ModifiedOn);
            return this.RedirectToAction("All", new { walletId = input.WalletId });
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
