namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface IBudgetsService
    {
        public Task<string> AddAsync(string userId, string name, DateTime startDate, DateTime endDate, decimal amount, string wallet);

        public Task<string> RemoveAsync(string userId, string name, string wallet);

        public Task<IEnumerable<BudgetInfoDto>> GetBudgetsAsync(string userId, string wallet);
    }
}
