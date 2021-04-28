namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface IRecordsService
    {
        public Task<string> AddAsync(string description, decimal amount, string category, string type, string wallet);

        public Task<string> RemoveAsync(int id);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(DateTime? startDate, DateTime endDate);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string category);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword);
    }
}
