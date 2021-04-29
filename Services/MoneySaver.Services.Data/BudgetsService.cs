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
    using MoneySaver.Services.Data.Models;

    public class BudgetsService : IBudgetsService
    {
        private readonly ApplicationDbContext dbContext;

        public BudgetsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string name, DateTime startDate, DateTime endDate, decimal amount, string wallet)
        {
            Wallet targetWallet = await this.dbContext.Wallets
                .FirstOrDefaultAsync(w => w.Name == wallet && w.ApplicationUser.Id == userId);

            if (targetWallet == null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.WalletNotExist, wallet));
            }

            Budget targetBudget = await this.dbContext.Budgets
                .FirstOrDefaultAsync(budget => budget.Name == name && budget.WalletId == targetWallet.Id);

            if (targetBudget != null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.BudgetAlreadyExists, name));
            }

            targetBudget.Name = name;
            targetBudget.StartDate = startDate;
            targetBudget.EndDate = endDate;
            targetBudget.Amount = amount;
            targetBudget.Wallet = targetWallet;

            await this.dbContext.Budgets.AddAsync(targetBudget);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.BudgetSuccessfullyAdded, targetBudget.Name);

            return successMessage;
        }

        public Task<IEnumerable<BudgetInfoDto>> GetBudgetsAsync(string userId, string wallet)
        {
            throw new NotImplementedException();
        }

        public Task<string> RemoveAsync(string userId, string name, string wallet)
        {
            throw new NotImplementedException();
        }
    }
}
