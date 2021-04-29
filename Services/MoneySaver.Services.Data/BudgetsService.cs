namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;

    public class BudgetsService : IBudgetsService
    {
        private readonly ApplicationDbContext dbContext;

        public BudgetsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string budgetName, DateTime startDate, DateTime endDate, decimal amount, string wallet)
        {
            Wallet targetWallet = await GetWalletByNameAsync(userId, wallet);

            if (targetWallet == null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.WalletNotExist, wallet));
            }

            Budget targetBudget = await this.dbContext.Budgets
                .FirstOrDefaultAsync(budget => budget.Name.ToLower() == budgetName.ToLower() && budget.WalletId == targetWallet.Id);

            if (targetBudget != null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.BudgetAlreadyExists, budgetName));
            }

            targetBudget.Name = budgetName;
            targetBudget.StartDate = startDate;
            targetBudget.EndDate = endDate;
            targetBudget.Amount = amount;
            targetBudget.Wallet = targetWallet;

            await this.dbContext.Budgets.AddAsync(targetBudget);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.BudgetSuccessfullyAdded, targetBudget.Name);

            return successMessage;
        }

        public async Task<IEnumerable<BudgetInfoDto>> GetAllBudgetsAsync(string userId, string wallet)
        {
            Wallet targetWallet = await GetWalletByNameAsync(userId, wallet);

            if (targetWallet == null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.WalletNotExist, wallet));
            }

            var budgets = await this.dbContext.Budgets
                .Where(budget => budget.Wallet.Name.ToLower() == wallet.ToLower() && budget.Wallet.ApplicationUserId == userId)
                .Select(budget => new BudgetInfoDto
                {
                    Wallet = budget.Wallet.Name,
                    Name = budget.Name,
                    Amount = budget.Amount,
                    StartDate = budget.StartDate.ToString(),
                    EndDate = budget.EndDate.ToString(),
                })
                .ToListAsync();

            return budgets;
        }

        public async Task<string> RemoveAsync(string userId, string budgetName, string wallet)
        {
            Budget budget = await this.dbContext.Budgets
                .FirstOrDefaultAsync(budget =>
                    budget.Name.ToLower() == budgetName.ToLower()
                    && budget.Wallet.Name.ToLower() == wallet.ToLower()
                    && budget.Wallet.ApplicationUserId == userId);

            if (budget == null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.InvalidBudgetName, budgetName));
            }

            this.dbContext.Budgets.Remove(budget);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.BudgetSuccessfullyRemoved, budget.Name);

            return successMessage;
        }

        public async Task<Wallet> GetWalletByNameAsync(string userId, string wallet)
        {
            Wallet targetWallet = await this.dbContext.Wallets
              .FirstOrDefaultAsync(w => w.Name.ToLower() == wallet.ToLower() && w.ApplicationUser.Id == userId);

            return targetWallet;
        }

        public async Task<BudgetInfoDto> GetBudgetByNameAsync(string userId, string budgetName, string walletName)
        {
            Wallet wallet = await GetWalletByNameAsync(userId, walletName);

            if (wallet == null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.WalletNotExist, wallet));
            }

            BudgetInfoDto targetBudget = await this.dbContext.Budgets
                .Select(budget => new BudgetInfoDto
                {
                    Wallet = budget.Wallet.Name,
                    Name = budget.Name,
                    Amount = budget.Amount,
                    StartDate = budget.StartDate.ToString(),
                    EndDate = budget.EndDate.ToString(),
                })
                .FirstOrDefaultAsync(budget =>
                    budget.Name.ToLower() == budgetName.ToLower()
                    && wallet.Name.ToLower() == budget.Wallet.ToLower()
                    && wallet.ApplicationUserId == userId);

            return targetBudget;
        }
    }
}
