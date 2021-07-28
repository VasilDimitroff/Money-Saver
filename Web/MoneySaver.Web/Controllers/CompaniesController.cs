namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Common;
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
                this.ViewBag.InvestmentWalletId = input.InvestmentWalletId;
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
                if (this.User.IsInRole(GlobalConstants.AdministratorRoleName))
                {
                    return this.Redirect($"/Companies/Index");
                }

                return this.Redirect($"/Investments/AllInvestments");
            }

            return this.Redirect($"/Trades/Add?investmentWalletId={input.InvestmentWalletId}");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Index()
        {
            var companiesDto = await this.companiesService.GetAllWithDeletedAsync();
            var companies = new List<CompanyViewModel>();

            companies = companiesDto.Select(c => new CompanyViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Ticker = c.Ticker,
                CreatedOn = c.CreatedOn,
                TradesCount = c.TradesCount,
            })
                .ToList();

            return this.View(companies);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id, [Bind("Ticker,Name,CreatedOn,ModifiedOn")] CompanyViewModel company)
        {
            if (id != company.Ticker)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(company);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            return this.View();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
           // var company = await this.dbContext.Companies.FindAsync(id);
           // this.dbContext.Companies.Remove(company);
           // await this.dbContext.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
