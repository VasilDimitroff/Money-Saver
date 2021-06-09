namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface IRecordsService
    {
        public Task<string> AddAsync(string description, string description1, decimal amount, int categoryId, string type, int walletId);

        public Task<string> RemoveAsync(string userId, int id, string wallet);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(string userId, DateTime? startDate, DateTime endDate, string wallet);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string userId, string category, string wallet);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string userId, string keyword, string wallet);
        void AddAsync(string userId, string description, decimal amount, string category, string type, string wallet);
    }
}
