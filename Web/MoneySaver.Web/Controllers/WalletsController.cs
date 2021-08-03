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
        private const int ItemsPerPage = 12;

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

        public async Task<IActionResult> AllWallets()
        {
            try
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
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Add()
        {
            try
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
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddWalletInputModel input)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    var currencies = await this.currenciesService.GetAllAsync();

                    input.Currencies = currencies
                    .Select(c => new CurrencyViewModel
                    {
                        Code = c.Code,
                        CurrencyId = c.CurrencyId,
                        Name = c.Name,
                    })
                    .ToList();

                    return this.View(input);
                }

                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.currenciesService.IsCurrencyExistAsync(input.CurrencyId))
                {
                    var currencies = await this.currenciesService.GetAllAsync();

                    input.Currencies = currencies
                    .Select(c => new CurrencyViewModel
                    {
                        Code = c.Code,
                        CurrencyId = c.CurrencyId,
                        Name = c.Name,
                    })
                    .ToList();

                    //this.TempData["CurrencyExist"] = "Please select a valid currency!";
                    //this.ViewBag.CurrencyExist = this.TempData["CurrencyExist"];
                    return this.View(input);
                }

                int walletId = await this.walletsService
                    .AddAsync(user.Id, input.Name, input.Amount, input.CurrencyId);

                return this.Redirect($"/Wallets/Details/{walletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
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
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditWalletViewModel input)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!this.ModelState.IsValid)
                {
                    if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.Id))
                    {
                        return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                    }

                    var walletInfo = await this.walletsService.GetWalletInfoForEditAsync(user.Id, input.Id);

                    input.Currencies = walletInfo.Currencies.Select(c => new CurrencyViewModel
                    {
                        Code = c.Code,
                        CurrencyId = c.CurrencyId,
                        Name = c.Name,
                    })
                        .ToList();

                    input.CurrencyId = walletInfo.CurrencyId;
                    input.Amount = walletInfo.Amount;
                    input.CurrentCurrencyName = walletInfo.CurrentCurrencyName;
                    input.CurrentCurrencyCode = walletInfo.CurrentCurrencyCode;
                    input.Name = walletInfo.Name;
                    input.Id = walletInfo.Id;

                    return this.View(input);
                }

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.Id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                await this.walletsService.EditAsync(user.Id, input.Id, input.Name, input.Amount, input.CurrencyId);

                return this.Redirect($"/Wallets/Details/{input.Id}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                await this.walletsService.RemoveAsync(id);

                return this.Redirect("/Wallets/AllWallets");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Records(int id, int page = 1)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                var model = new WalletSearchResultViewModel();

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
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
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
                        CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                        Description = r.Description,
                        Id = r.Id,
                    }),
                };

                var incomesLast30Days = await this.walletsService.GetWalletCategoriesIncomesLast30DaysAsync(id);

                model.MonthIncomes = incomesLast30Days.Select(c => new CategoryIncomesLast30DaysWalletDetailsViewModel
                {
                    BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor.ToString()),
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    TotalIncomesLast30days = c.TotalIncomesLast30Days,
                    TotalIncomeRecordsLast30Days = c.TotalIncomeRecordsLast30Days,
                })
                    .ToList();

                var outcomesLast30Days = await this.walletsService.GetWalletCategoriesExpensesLast30DaysAsync(id);

                model.MonthExpenses = outcomesLast30Days.Select(c => new CategoryExpensesLast30DaysWalletDetailsViewModel
                {
                    BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor.ToString()),
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    TotalExpensesLast30Days = c.TotalExpensesLast30Days,
                    TotalExpenseRecordsLast30Days = c.TotalExpenseRecordsLast30Days,
                })
                    .ToList();

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Search(int id, string searchTerm, int page = 1)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                if (searchTerm == null)
                {
                    searchTerm = string.Empty;
                }

                var model = new WalletSearchResultViewModel();

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
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> DateSorted(DateTime startDate, DateTime endDate, int id, int page = 1)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                var model = new WalletSearchResultViewModel();

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
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Categories(int id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                var dbResult = await this.walletsService.GetWalletWithCategoriesAsync(id);

                StatisticsWalletViewModel model = new StatisticsWalletViewModel()
                {
                    WalletId = dbResult.WalletId,
                    WalletName = dbResult.WalletName,
                    Categories = dbResult.Categories.Select(c => new CategoryStatisticsViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor),
                        TotalRecordsCount = c.RecordsCount,
                        TotalExpensesAmount = c.TotalExpensesAmount,
                        TotalIncomesAmount = c.TotalIncomesAmount,
                        ModifiedOn = c.ModifiedOn == null ? "No records yet" : c.ModifiedOn.Value.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                    })
                    .ToList(),
                    Incomes = dbResult.Incomes,
                    Outcomes = dbResult.Outcomes,
                    Currency = dbResult.Currency,
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }
    }
}
