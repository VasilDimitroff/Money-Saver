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

        public async Task<string> AddAsync(string description, decimal amount, string category, string type, string wallet)
        {
            var targetWallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Name == wallet);
            RecordType recordType;
            bool isTypeParsed = Enum.TryParse<RecordType>(type, out recordType);
            Category targetCategory = await this.dbContext.Categories.FirstOrDefaultAsync(categ => categ.Name == category);

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

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string category)
        {
            Category targetCategory = await this.dbContext.Categories.FirstOrDefaultAsync(categ => categ.Name == category);

            if (targetCategory == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfCategory);
            }

            var records = await this.dbContext.Records
                .Where(record => record.Category.Name == category)
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

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(DateTime? startDate, DateTime endDate)
        {
            if (startDate == null)
            {
               startDate = endDate.AddDays(-7);
            }

            var records = await this.dbContext.Records
                .Where(record => record.CreatedOn >= startDate && record.CreatedOn <= endDate)
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

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword)
        {
            if (keyword == null)
            {
                var records = await this.dbContext.Records
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
                    .Where(x => x.Description.Contains(keyword))
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

        public async Task<string> RemoveAsync(int id)
        {
            Record targetRecord = await this.dbContext.Records.FirstOrDefaultAsync(record => record.Id == id);
            if (targetRecord == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordId);
            }

            string successfullMessage = GlobalConstants.SuccessfullyRemovedRecord;

            return successfullMessage;
        }
    }
}
