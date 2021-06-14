namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Records;
    using MoneySaver.Web.Models.Records.Enums;
    using MoneySaver.Web.Models.Wallets;

    public class WalletsController : Controller
    {
        private readonly IWalletsService walletsService;
        private readonly IRecordsService recordsService;

        public WalletsController(IWalletsService walletsService, IRecordsService recordsService)
        {
            this.walletsService = walletsService;
            this.recordsService = recordsService;
        }

        /*
        public Task<IActionResult> All()
        {
            return View();
        }

        public Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public Task<IActionResult> Add()
        {
            return View();
        }

        public Task<IActionResult> Edit()
        {
            return View();
        }

        [HttpPost]
        public Task<IActionResult> Edit(int id)
        {
            return View();
        }

        public Task<IActionResult> Delete()
        {
            return View();
        }

        [HttpPost]
        public Task<IActionResult> Delete(int id)
        {
            return View();
        }

        public Task<IActionResult> Details(int id)
        {
            return View();
        }
        */
        public async Task<IActionResult> Records(int id)
        {
            var model = new WalletSearchResultViewModel();

            var records = await this.recordsService.GetRecordsByWalletAsync(id);

            model.Records = records.Select(r => new WalletSearchResultSingleRecordViewModel
            {
                Amount = r.Amount,
                Category = r.Category,
                CategoryId = r.CategoryId,
                CreatedOn = r.CreatedOn.ToString("D", CultureInfo.InvariantCulture),
                ModifiedOn = r.ModifiedOn.HasValue ? r.ModifiedOn.Value.ToString("D", CultureInfo.InvariantCulture) : null,
                Description = r.Description,
                Wallet = r.Wallet,
                Id = r.Id,
                Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                Currency = r.Currency,
            })
                .ToList();

            model.WalletId = id;
            model.Wallet = await this.walletsService.GetWalletNameAsync(id);

            return this.View(model);
        }

        public async Task<IActionResult> Search(int id, string searchTerm)
        {
            if (searchTerm == null)
            {
                searchTerm = string.Empty;
            }

            var model = new WalletSearchResultViewModel();

            var records = await this.walletsService.GetRecordsByKeywordAsync(searchTerm, id);

            model.Records = records.Select(r => new WalletSearchResultSingleRecordViewModel
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
            model.WalletId = id;
            model.Wallet = await this.walletsService.GetWalletNameAsync(id);

            return this.View("Records", model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dtoResult = await this.walletsService.GetWalletDetailsAsync(id);
            WalletDetailsViewModel model = new WalletDetailsViewModel
            {
                Currency = dtoResult.Currency,
                CurrentBalance = dtoResult.CurrentBalance,
                WalletId = dtoResult.WalletId,
                WalletName = dtoResult.WalletName,
                TotalWalletExpensesLast30Days = dtoResult.TotalWalletExpensesLast30Days,
                TotalWalletIncomesLast30Days = dtoResult.TotalWalletIncomesLast30Days,
                TotalRecordsCountLast30Days = dtoResult.TotalRecordsCountLast30Days,
                Categories = dtoResult.Categories.Select(c => new WalletDetailsCategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    RecordsCount = c.RecordsCount,
                    TotalExpenses = c.TotalExpenses,
                    TotalIncomes = c.TotalIncomes,
                }),
                Records = dtoResult.Records
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new WalletDetailsRecordViewModel
                {
                    CategoryId = r.CategoryId,
                    CategoryName = r.CategoryName,
                    Amount = r.Amount,
                    CreatedOn = r.CreatedOn.ToString("D", CultureInfo.InvariantCulture),
                    Description = r.Description,
                    Id = r.Id,
                }),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DateSorted(DateTime startDate, DateTime endDate, int id)
        {
            var model = new WalletSearchResultViewModel();

            var records = await this.walletsService.GetRecordsByDateRangeAsync(startDate, endDate, id);

            model.Records = records.Select(r => new WalletSearchResultSingleRecordViewModel
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

            model.WalletId = id;
            model.Wallet = await this.walletsService.GetWalletNameAsync(id);

            return this.View("Records", model);
        }

        public async Task<IActionResult> Statistics(int id)
        {
            var dbResult = await this.walletsService.GetWalletWithCategoriesAsync(id);

            StatisticsWalletViewModel model = new StatisticsWalletViewModel()
            {
                WalletId = dbResult.WalletId,
                ModifiedOn = dbResult.ModifiedOn.HasValue ? dbResult.ModifiedOn.Value.ToString("D", CultureInfo.InvariantCulture) : null,
                TotalAmount = dbResult.TotalAmount,
                WalletName = dbResult.WalletName,
                Categories = dbResult.Categories.Select(c => new CategoryStatisticsViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalRecordsCount = c.RecordsCount,
                    TotalExpensesAmount = c.TotalExpensesAmount,
                    TotalIncomesAmount = c.TotalIncomesAmount,
                })
                .ToList(),
                Incomes = dbResult.Incomes,
                Outcomes = dbResult.Outcomes,
                Currency = dbResult.Currency,
            };

            return this.View(model);
        }
    }
}
