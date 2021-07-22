namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Companies;

    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ICompaniesService companiesService;

        public CompaniesController(ICompaniesService companiesService)
        {
            this.companiesService = companiesService;
        }

        public IActionResult Add(int investmentWalletId = 0)
        {
            this.ViewBag.InvestmentWalletId = investmentWalletId;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCompanyInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            if (await this.companiesService.IsCompanyAlreadyExistAsync(input.Ticker))
            {
                this.TempData["CompanyExist"] = "Company with this ticker already exists!";
                this.ViewBag.InvestmentWalletId = input.InvestmentWalletId;
                return this.View(input);
            }

            await this.companiesService.AddAsync(input.Ticker, input.CompanyName);

            if (input.InvestmentWalletId == 0)
            {
                return this.Redirect($"/Investments/AllInvestments");
            }

            return this.Redirect($"/Trades/Add?investmentWalletId={input.InvestmentWalletId}");
        }
    }
}
