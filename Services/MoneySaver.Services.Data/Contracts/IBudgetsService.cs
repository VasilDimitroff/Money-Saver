namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MoneySaver.Services.Data.Models;

    public interface IBudgetsService
    {
        public Task<string> AddAsync(int walletId, string name, DateTime startDate, DateTime endDate, decimal amount);

        public Task<string> RemoveAsync(int budgetId);

        public Task<IEnumerable<BudgetInfoDto>> GetAllBudgetsAsync(string userId, int walletId);

        public Task<BudgetInfoDto> GetBudgetByIdAsync(int budgetId);
    }
}
