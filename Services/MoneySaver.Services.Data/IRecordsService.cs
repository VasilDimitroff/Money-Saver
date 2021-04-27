namespace MoneySaver.Services.Data
{
    using MoneySaver.Services.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRecordsService
    {
        public Task<string> Add(string description, decimal amount, string category, string type, string wallet);

        public Task<string> Remove(int id);

        public Task<RecordInfoDto> GetRecordsByDateRange(DateTime startDate, DateTime endDate);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByCategory(string category);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordInDateRange(string keyword, DateTime startDate, DateTime endDate);
    }
}
