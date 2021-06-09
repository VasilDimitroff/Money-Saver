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

        public Task<string> AddAsync(int walletId, string name, DateTime startDate, DateTime endDate, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BudgetInfoDto>> GetAllBudgetsAsync(string userId, int walletId)
        {
            throw new NotImplementedException();
        }

        public Task<BudgetInfoDto> GetBudgetByIdAsync(int budgetId)
        {
            throw new NotImplementedException();
        }

        public Task<string> RemoveAsync(int budgetId)
        {
            throw new NotImplementedException();
        }
    }
}
