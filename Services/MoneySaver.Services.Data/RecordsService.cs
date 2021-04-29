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
    using MoneySaver.Services.Data.Models;

    public class RecordsService : IRecordsService
    {
        private readonly ApplicationDbContext dbContext;

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string description, decimal amount, string category, string type, string wallet)
        {
            var targetWallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Name.ToLower() == wallet.ToLower() && w.ApplicationUserId == userId);
            RecordType recordType;
            bool isTypeParsed = Enum.TryParse<RecordType>(type, out recordType);

            Category targetCategory = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Name.ToLower() == category.ToLower() && categ.Records.Any(r => r.Wallet.ApplicationUserId == userId));

            if (targetCategory == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfCategory);
            }

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            if (!isTypeParsed)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordType);
            }

            Record record = new Record()
            {
                Description = description,
                Amount = amount,
                Category = targetCategory,
                Type = recordType,
                Wallet = targetWallet,
            };

            await this.dbContext.Records.AddAsync(record);
            await this.dbContext.SaveChangesAsync();
            return string.Format(GlobalConstants.SuccessfullyAddedRecord, description, recordType, amount, targetCategory);
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string userId, string category)
        {
            Category targetCategory = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Name.ToLower() == category.ToLower() && (categ.Records.Any(r => r.Wallet.ApplicationUserId == userId)));

            if (targetCategory == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfCategory);
            }

            var records = await this.dbContext.Records
                .Where(record => record.Category.Name.ToLower() == category.ToLower())
                .Select(record => new RecordInfoDto
                {
                    Category = record.Category.Name,
                    Amount = record.Amount,
                    Description = record.Description,
                    Type = record.Type.ToString(),
                    Wallet = record.Wallet.Name,
                })
                .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(string userId, DateTime? startDate, DateTime endDate)
        {
            if (startDate == null)
            {
               startDate = endDate.AddDays(-7);
            }

            var records = await this.dbContext.Records
                .Where(record => record.CreatedOn >= startDate && record.CreatedOn <= endDate && record.Wallet.ApplicationUserId == userId)
                .Select(record => new RecordInfoDto
                {
                    Category = record.Category.Name,
                    Amount = record.Amount,
                    Description = record.Description,
                    Type = record.Type.ToString(),
                    Wallet = record.Wallet.Name,
                })
                .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string userId, string keyword)
        {
            if (keyword == null)
            {
                var records = await this.dbContext.Records
                    .Where(record => record.Wallet.ApplicationUserId == userId)
                    .Select(record => new RecordInfoDto
                    {
                        Category = record.Category.Name,
                        Amount = record.Amount,
                        Description = record.Description,
                        Type = record.Type.ToString(),
                        Wallet = record.Wallet.Name,
                    })
                    .ToListAsync();

                return records;
            }

            var filteredRecords = await this.dbContext.Records
                    .Where(x => x.Description.ToLower().Contains(keyword.ToLower()) && x.Wallet.ApplicationUserId == userId)
                    .Select(record => new RecordInfoDto
                    {
                        Category = record.Category.Name,
                        Amount = record.Amount,
                        Description = record.Description,
                        Type = record.Type.ToString(),
                        Wallet = record.Wallet.Name,
                    })
                    .ToListAsync();

            return filteredRecords;
        }

        public async Task<string> RemoveAsync(string userId, int id)
        {
            Record targetRecord = await this.dbContext.Records
                .FirstOrDefaultAsync(record => record.Id == id && record.Wallet.ApplicationUserId == userId);

            if (targetRecord == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordId);
            }

            string successfullMessage = GlobalConstants.SuccessfullyRemovedRecord;

            return successfullMessage;
        }
    }
}
