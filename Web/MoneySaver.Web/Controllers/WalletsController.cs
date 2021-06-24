namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Common;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;
    using MoneySaver.Web.ViewModels.Wallets;

    [Authorize]
    public class WalletsController : Controller
    {
        private readonly IWalletsService walletsService;
        private readonly IRecordsService recordsService;
        private readonly ICurrenciesService currenciesService;
        private readonly UserManager<ApplicationUser> userManager;

        public WalletsController(
            IWalletsService walletsService,
            IRecordsService recordsService,
            ICurrenciesService currenciesService,
            UserManager<ApplicationUser> userManager)
        {
            this.walletsService = walletsService;
            this.recordsService = recordsService;
            this.currenciesService = currenciesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> All()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var wallets = await this.walletsService.GetAllWalletsAsync(user.Id);

            var modelWallets = new List<AllWalletsViewModel>();

            modelWallets = wallets.Select(w => new AllWalletsViewModel
            {
                CurrentBalance = w.CurrentBalance,
                TotalExpenses = w.TotalExpenses,
                TotalIncomes = w.TotalIncomes,
                WalletId = w.WalletId,
                WalletName = w.WalletName,
                Currency = w.Currency,
            })
                .ToList();

            return this.View(modelWallets);
        }

        public async Task<IActionResult> Add()
        {
            AddWalletInputModel model = new AddWalletInputModel()
            {
                Amount = 0,
                Name = string.Empty,
            };

            var currencies = await this.currenciesService.GetAllAsync();

            model.Currencies = currencies
                .Select(c => new CurrencyViewModel
                {
                    Code = c.Code,
                    CurrencyId = c.CurrencyId,
                    Name = c.Name,
                })
                .ToList();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddWalletInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.walletsService
                .AddAsync(user.Id, input.Name, input.Amount, input.CurrencyId);

            return this.Redirect("/Wallets/All");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            var model = new EditWalletViewModel();

            var walletInfo = await this.walletsService.GetWalletInfoForEditAsync(user.Id, id);

            model.Currencies = walletInfo.Currencies.Select(c => new CurrencyViewModel
            {
                Code = c.Code,
                CurrencyId = c.CurrencyId,
                Name = c.Name,
            })
                .ToList();

            model.CurrencyId = walletInfo.CurrencyId;
            model.Amount = walletInfo.Amount;
            model.CurrentCurrencyName = walletInfo.CurrentCurrencyName;
            model.CurrentCurrencyCode = walletInfo.CurrentCurrencyCode;
            model.Name = walletInfo.Name;
            model.Id = walletInfo.Id;

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditWalletViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.Id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            await this.walletsService.EditAsync(user.Id, input.Id, input.Name, input.Amount, input.CurrencyId);

            return this.Redirect($"/Wallets/All/");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            await this.walletsService.RemoveAsync(id);

            return this.Redirect("/Wallets/All");
        }

        public async Task<IActionResult> Records(int id, int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            var model = new WalletSearchResultViewModel();

            const int ItemsPerPage = 12;

            var records = await this.walletsService.GetAllRecordsAsync(page, id, ItemsPerPage);

            model.ItemsPerPage = ItemsPerPage;
            model.PageNumber = page;
            model.RecordsCount = this.walletsService.GetCount(id);

            model.Records = records.Select(r => new WalletSearchResultSingleRecordViewModel
            {
                Amount = r.Amount,
                Category = r.Category,
                CategoryId = r.CategoryId,
                CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                ModifiedOn = r.ModifiedOn.HasValue ? r.ModifiedOn.Value.ToString(CultureInfo.InvariantCulture) : null,
                Description = r.Description,
                Wallet = r.Wallet,
                Id = r.Id,
                Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                Currency = r.Currency,
                BadgeColor = Enum.Parse<BadgeColor>(r.BadgeColor.ToString()),
            })
                .ToList();

            model.WalletId = id;
            model.Wallet = await this.walletsService.GetWalletNameAsync(id);

            return this.View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            var dtoResult = await this.walletsService.GetWalletDetailsAsync(user.Id, id);
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
                    BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor.ToString()),
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
                    CategoryBadgeColor = Enum.Parse<BadgeColor>(r.CategoryBadgeColor.ToString()),
                    Amount = r.Amount,
                    CreatedOn = r.CreatedOn.ToString("M", CultureInfo.InvariantCulture),
                    Description = r.Description,
                    Id = r.Id,
                }),
            };

            return this.View(model);
        }

        public async Task<IActionResult> Search(int id, string searchTerm, int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            if (searchTerm == null)
            {
                searchTerm = string.Empty;
            }

            var model = new WalletSearchResultViewModel();

            const int ItemsPerPage = 12;

            model.ItemsPerPage = ItemsPerPage;
            model.PageNumber = page;

            var records = await this.walletsService.GetRecordsByKeywordAsync(searchTerm, id, page, ItemsPerPage);

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
                BadgeColor = Enum.Parse<BadgeColor>(r.BadgeColor.ToString()),
            })
                .ToList();

            model.RecordsCount = this.walletsService.GetSearchRecordsCount(searchTerm, id);
            model.SearchTerm = searchTerm;
            model.WalletId = id;
            model.Wallet = await this.walletsService.GetWalletNameAsync(id);

            return this.View(model);
        }

        public async Task<IActionResult> DateSorted(DateTime startDate, DateTime endDate, int id, int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            var model = new WalletSearchResultViewModel();

            const int ItemsPerPage = 12;

            model.ItemsPerPage = ItemsPerPage;
            model.PageNumber = page;

            var records = await this.walletsService.GetRecordsByDateRangeAsync(startDate, endDate, id, page, ItemsPerPage);

            model.Records = records.Select(r => new WalletSearchResultSingleRecordViewModel
            {
                Amount = r.Amount,
                Category = r.Category,
                CategoryId = r.CategoryId,
                CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                Description = r.Description,
                Wallet = r.Wallet,
                Id = r.Id,
                Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                Currency = r.Currency,
                BadgeColor = Enum.Parse<BadgeColor>(r.BadgeColor.ToString()),
            })
                .ToList();

            model.RecordsCount = this.walletsService.GetDateSortedRecordsCount(startDate, endDate, id);
            model.WalletId = id;
            model.Wallet = await this.walletsService.GetWalletNameAsync(id);

            var startDate12AM = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate12PM = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            model.StartDate = startDate12AM;
            model.EndDate = endDate12PM;

            return this.View(model);
        }

        public async Task<IActionResult> Categories(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            var dbResult = await this.walletsService.GetWalletWithCategoriesAsync(id);

            StatisticsWalletViewModel model = new StatisticsWalletViewModel()
            {
                WalletId = dbResult.WalletId,
                ModifiedOn = dbResult.ModifiedOn.HasValue ? dbResult.ModifiedOn.Value.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture) : null,
                TotalAmount = dbResult.TotalAmount,
                WalletName = dbResult.WalletName,
                Categories = dbResult.Categories.Select(c => new CategoryStatisticsViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor),
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
