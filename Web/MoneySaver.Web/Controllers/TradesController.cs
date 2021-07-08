﻿namespace MoneySaver.Web.Controllers
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
    using MoneySaver.Web.ViewModels.Trades;

    [Authorize]
    public class TradesController : Controller
    {
        private readonly ICompaniesService companiesService;
        private readonly ITradesService tradesService;
        private readonly UserManager<ApplicationUser> userManager;

        public TradesController(
            ICompaniesService companiesService,
            ITradesService tradesService,
            UserManager<ApplicationUser> userManager)
        {
            this.companiesService = companiesService;
            this.tradesService = tradesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Buy()
        {
            var companies = await this.companiesService.GetAllCompaniesAsync();
            var model = new BuyStockInputModel
            {
                Companies = companies.Select(c => new CompanyViewModel
                {
                    Name = c.Name,
                    Ticker = c.Ticker,
                })
                .ToList(),
            };

            var selectedCompany = new CompanyViewModel();
            model.SelectedCompany = selectedCompany;

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Buy(BuyStockInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.tradesService.CreateBuyTradeAsync(user.Id, input.SelectedCompany.Ticker, input.Quantity, input.Price);

            return this.Redirect("/Trades/All");
        }
    }
}
