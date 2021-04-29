namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Models;

    public interface IBudgetsService
    {
        public Task<string> AddAsync(string userId, string budgetName, DateTime startDate, DateTime endDate, decimal amount, string wallet);

        public Task<string> RemoveAsync(string userId, string budgetName, string wallet);

        public Task<IEnumerable<BudgetInfoDto>> GetAllBudgetsAsync(string userId, string wallet);

        public Task<BudgetInfoDto> GetBudgetByNameAsync(string userId, string budgetName, string walletName);
    }
}
