using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Data;
using MoneySaver.Data.Models;

namespace MoneySaver.Web.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CompaniesController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        // GET: Administration/Companies
        public async Task<IActionResult> Index()
        {
            return this.View(await this.dbContext.Companies.ToListAsync());
        }

        // GET: Administration/Companies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var company = await this.dbContext.Companies
                .FirstOrDefaultAsync(m => m.Ticker == id);
            if (company == null)
            {
                return this.NotFound();
            }

            return this.View(company);
        }

        // GET: Administration/Companies/Create
        public IActionResult Create()
        {
            return this.View();
        }

        // POST: Administration/Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ticker,Name,CreatedOn,ModifiedOn")] Company company)
        {
            if (this.ModelState.IsValid)
            {
                this.dbContext.Add(company);
                await this.dbContext.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }
            return this.View(company);
        }

        // GET: Administration/Companies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var company = await this.dbContext.Companies.FindAsync(id);
            if (company == null)
            {
                return this.NotFound();
            }

            return this.View(company);
        }

        // POST: Administration/Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Ticker,Name,CreatedOn,ModifiedOn")] Company company)
        {
            if (id != company.Ticker)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.dbContext.Update(company);
                    await this.dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.CompanyExists(company.Ticker))
                    {
                        return this.NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(company);
        }

        // GET: Administration/Companies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var company = await this.dbContext.Companies
                .FirstOrDefaultAsync(m => m.Ticker == id);
            if (company == null)
            {
                return this.NotFound();
            }

            return this.View(company);
        }

        // POST: Administration/Companies/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var company = await this.dbContext.Companies.FindAsync(id);
            this.dbContext.Companies.Remove(company);
            await this.dbContext.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        private bool CompanyExists(string id)
        {
            return this.dbContext.Companies.Any(e => e.Ticker == id);
        }
    }
}
