using Microsoft.EntityFrameworkCore;
using MoneySaver.Data;
using MoneySaver.Data.Models;
using MoneySaver.Data.Models.Enums;
using MoneySaver.Services.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Services.Data
{
    public class RecordsService : IRecordsService
    {
        private readonly ApplicationDbContext dbContext;

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> Add(string description, decimal amount, string category, string type, string wallet)
        {
            var targetWallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.Name == wallet);
            RecordType recordType;
            bool isTypeParsed = Enum.TryParse<RecordType>(type, out recordType);
            Category targetCategory = await dbContext.Categories.FirstOrDefaultAsync(categ => categ.Name == category);

            if (targetWallet == null || targetCategory == null)
            {
                throw new ArgumentNullException("Wallet or Category Cannot Be Null");
            }

            if (!isTypeParsed)
            {
                throw new ArgumentException("Invalid Record Type");
            }

            Record record = new Record()
            {
                Description = description,
                Amount = amount,
                Category = targetCategory,
                Type = recordType,
                Wallet = targetWallet,
            };

            await dbContext.Records.AddAsync(record);
            await dbContext.SaveChangesAsync();

            return $"Record {description} with type {recordType} and amount {amount} successfully added in category {targetCategory}";
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Task<RecordInfoDto> GetRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordInDateRange(string keyword, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<string> Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
