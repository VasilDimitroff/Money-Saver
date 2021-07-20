﻿namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels;
    using MoneySaver.Web.ViewModels.Home;
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class HomeController : BaseController
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService homeService)
        {
            this.homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = string.Empty;
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var userIdentifier = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                userId = userIdentifier;
            }
            catch (Exception)
            {
                return this.RedirectToAction(nameof(this.Error), new { Message = "You must to log in first!" });
            }

            var dto = await this.homeService.GetIndexInfoAsync(userId);

            var model = new IndexViewModel()
            {
                CategoriesLast30DaysExpenses = dto.CategoriesLast30DaysExpenses.Select(ce => new AccountCategoryExpensesLast30DaysViewModel
                {
                    CategoryId = ce.CategoryId,
                    BadgeColor = Enum.Parse<BadgeColor>(ce.BadgeColor.ToString()),
                    CategoryName = ce.CategoryName,
                    WalletId = ce.WalletId,
                    WalletName = ce.WalletName,
                    CurrencyCode = ce.CurrencyCode,
                    TotalExpenseRecordsLast30Days = ce.TotalExpenseRecordsLast30Days,
                    TotalExpensesLast30Days = ce.TotalExpensesLast30Days,
                })
                .ToList(),
                CategoriesLast30DaysIncomes = dto.CategoriesLast30DaysIncomes.Select(ci => new AccountCategoryIncomesLast30DaysViewModel
                {
                    BadgeColor = Enum.Parse<BadgeColor>(ci.BadgeColor.ToString()),
                    CategoryId = ci.CategoryId,
                    CategoryName = ci.CategoryName,
                    WalletId = ci.WalletId,
                    WalletName = ci.WalletName,
                    CurrencyCode = ci.CurrencyCode,
                    TotalIncomeRecordsLast30Days = ci.TotalIncomeRecordsLast30Days,
                    TotalIncomesLast30days = ci.TotalIncomesLast30Days,
                })
                .ToList(),
                ActiveToDoLists = dto.ActiveToDoLists.Select(l => new IndexListViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                })
                .ToList(),
                Wallets = dto.Wallets.Select(w => new IndexWalletViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    CurrencyCode = w.CurrencyCode,
                    Amount = w.Amount,
                })
                .ToList(),
            };

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message = "Error happened!")
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier, Message = message });
        }
    }
}
