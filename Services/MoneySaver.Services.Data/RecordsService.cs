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
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;

    public class RecordsService : IRecordsService
    {
        private readonly ApplicationDbContext dbContext;

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<string> AddAsync(string description, string description1, decimal amount, int categoryId, string type, int walletId)
        {
            throw new NotImplementedException();
        }

        public void AddAsync(string userId, string description, decimal amount, string category, string type, string wallet)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string userId, string category, string wallet)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(string userId, DateTime? startDate, DateTime endDate, string wallet)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string userId, string keyword, string wallet)
        {
            throw new NotImplementedException();
        }

        public Task<string> RemoveAsync(string userId, int id, string wallet)
        {
            throw new NotImplementedException();
        }
    }
}
