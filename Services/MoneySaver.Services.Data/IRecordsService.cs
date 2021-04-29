namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface IRecordsService
    {
        public Task<string> AddAsync(string userId, string description, decimal amount, string category, string type, string wallet);

        public Task<string> RemoveAsync(string userId, int id);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(string userId, DateTime? startDate, DateTime endDate);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string userId, string category);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string userId, string keyword);
    }
}
