namespace MoneySaver.Services.Data.Tests
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
    using MoneySaver.Services.Data.Models.Records;
    using Moq;
    using Xunit;

    public class RecordsServiceTests
    {
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private IRecordsService recService;

        public RecordsServiceTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("recordsDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.recService = new RecordsService(this.db);
        }

        [Fact]
        public async Task AddAsyncShoudSuccessfullyAddRecord()
        {
            this.FillDatabase();

            // Act
            string result = await this.recService.AddAsync(4, "test record", 2, "Expense", DateTime.UtcNow);

            var addedRecord = this.db.Records.FirstOrDefault(r => r.Description.Contains("test"));

            // Assert
            Assert.Equal(GlobalConstants.RecordSuccessfullyAdded, result);
            Assert.Equal(RecordType.Expense, addedRecord.Type);
            Assert.Equal(4, addedRecord.CategoryId);
            Assert.Equal("test record", addedRecord.Description);
            Assert.Equal(-2, addedRecord.Amount);
        }

        [Fact]
        public async Task DescriptionShoudBeDefaultWhenRecordDescriptionIsNull()
        {
            this.FillDatabase();

            // Act
            string result = await this.recService.AddAsync(4, null, 2, "Expense", DateTime.UtcNow);

            var addedRecord = this.db.Records.OrderByDescending(r => r.CreatedOn).FirstOrDefault();

            // Assert
            Assert.Equal(GlobalConstants.RecordSuccessfullyAdded, result);
            Assert.Equal(RecordType.Expense, addedRecord.Type);
            Assert.Equal(4, addedRecord.CategoryId);
            Assert.Equal("N/A", addedRecord.Description);
            Assert.Equal(-2, addedRecord.Amount);
        }

        [Fact]
        public async Task DescriptionShoudBeDefaultWhenRecordDescriptionIsEmptyString()
        {
            this.FillDatabase();

            // Act
            string result = await this.recService.AddAsync(4, string.Empty, -2, "Income", DateTime.UtcNow);

            var addedRecord = this.db.Records.OrderByDescending(r => r.CreatedOn).FirstOrDefault();

            // Assert
            Assert.Equal(GlobalConstants.RecordSuccessfullyAdded, result);
            Assert.Equal(RecordType.Income, addedRecord.Type);
            Assert.Equal(4, addedRecord.CategoryId);
            Assert.Equal("N/A", addedRecord.Description);
            Assert.Equal(2, addedRecord.Amount);
        }

        [Fact]
        public async Task WalletAmountShouldDecreaseWithValueOfRecordWhenRecordIsAdded()
        {
            this.FillDatabase();

            // Act
            string result = await this.recService.AddAsync(4, "Party with girls", 25, "Expense", DateTime.UtcNow);

            var addedRecord = this.db.Records.OrderByDescending(r => r.CreatedOn).FirstOrDefault();

            var walletAmount = this.db.Wallets.FirstOrDefault(w => w.Id == 5).MoneyAmount;

            // Assert
            Assert.Equal(GlobalConstants.RecordSuccessfullyAdded, result);
            Assert.Equal(RecordType.Expense, addedRecord.Type);
            Assert.Equal(4, addedRecord.CategoryId);
            Assert.Equal("Party with girls", addedRecord.Description);
            Assert.Equal(-25, addedRecord.Amount);
            Assert.Equal(475, walletAmount);
        }

        [Fact]
        public async Task WalletAmountShouldIncreaseWithValueOfRecordWhenRecordIsAdded()
        {
            this.FillDatabase();

            // Act
            string result = await this.recService.AddAsync(4, "Salary", 1000, "Income", DateTime.UtcNow);

            var addedRecord = this.db.Records.OrderByDescending(r => r.CreatedOn).FirstOrDefault();

            var walletAmount = this.db.Wallets.FirstOrDefault(w => w.Id == 5).MoneyAmount;

            // Assert
            Assert.Equal(GlobalConstants.RecordSuccessfullyAdded, result);
            Assert.Equal(RecordType.Income, addedRecord.Type);
            Assert.Equal(4, addedRecord.CategoryId);
            Assert.Equal("Salary", addedRecord.Description);
            Assert.Equal(1000, addedRecord.Amount);
            Assert.Equal(1500, walletAmount);
        }

        [Fact]
        public void AddRecordsShouldThrowsExceptionWhenCategoryIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => this.recService.AddAsync(40, "new record", 5, "Expense", DateTime.UtcNow));
        }

        [Fact]
        public void AddRecordsShouldThrowsExceptionWhenRecordTypeIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => this.recService.AddAsync(4, string.Empty, 5, "InvalidRecordType", DateTime.UtcNow));
        }

        [Fact]
        public async Task RecordsCountShouldBe2WhenRemoveAsyncIsExecuted()
        {
            this.FillDatabase();
            var record = this.db.Records.FirstOrDefault(r => r.Id == "record1");

            // Act
            string result = await this.recService.RemoveAsync("record1");

            var removedRecord = this.db.Records.FirstOrDefault(r => r.Id == "record1");
            var recordsCount = this.db.Records.Where(w => w.CategoryId == 4).Count();

            // Assert
            Assert.Null(removedRecord);
            Assert.Equal(2, recordsCount);
        }

        [Fact]
        public async Task WalletAmountShoudIncreaseWith5WhenRecordIsRemoved()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.RemoveAsync("record1");

            var removedRecord = this.db.Records.FirstOrDefault(r => r.Id == "record1");
            int recordsCount = this.db.Records.Where(w => w.CategoryId == 4).Count();
            decimal walletAmount = this.db.Wallets.FirstOrDefault(x => x.Id == 5).MoneyAmount;

            // Assert
            Assert.Null(removedRecord);
            Assert.Equal(2, recordsCount);
            Assert.Equal(505, walletAmount);
        }

        [Fact]
        public async Task WalletAmountShoudDecreaseWith15WhenRecordIsRemoved()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.RemoveAsync("record3");

            var removedRecord = this.db.Records.FirstOrDefault(r => r.Id == "record3");
            int recordsCount = this.db.Records.Where(w => w.CategoryId == 4).Count();
            decimal walletAmount = this.db.Wallets.FirstOrDefault(x => x.Id == 5).MoneyAmount;

            // Assert
            Assert.Null(removedRecord);
            Assert.Equal(2, recordsCount);
            Assert.Equal(485, walletAmount);
        }

        [Fact]
        public void RemoveRecordShouldThrowsExceptionWhenRecordIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => this.recService.RemoveAsync("invalidRecordId"));
        }

        [Fact]
        public async Task CategoriesCountShouldBe1WhenMethodIsExecuted()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.recService.GetRecordWithAllCategories("record1", 5);
            var categoryName = result.Categories.FirstOrDefault().Name;
            // Assert
            Assert.Equal(1, result.Categories.Count());
            Assert.Equal("Parties", categoryName);
        }

        [Fact]
        public async Task MethodShouldReturnValidObject()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.recService.GetRecordWithAllCategories("record1", 5);

            var recordDescription = result.Description;
            var recordAmount = result.Amount;
            var recordType = result.Type;
            var recordWalletName = result.WalletName;

            // Assert
            Assert.Equal("Holiday Wallet", recordWalletName);
            Assert.Equal("Party in the club", recordDescription);
            Assert.Equal(-5, recordAmount);
            Assert.Equal(RecordType.Expense, recordType);
        }

        [Fact]
        public async Task GetRecordWithAllCategoriesShouldThrowsExceptionWhenRecordIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.recService.GetRecordWithAllCategories("invalidRecordId", 5));
        }

        [Fact]
        public async Task NewRecordShouldBeSuccessfullyUpdatedWhenRecordUpdateMethodIsExecuted()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record1", 4, 5, "updated description", -5, 20, "Expense", DateTime.UtcNow);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record1");
            var recordDescription = record.Description;
            var recordAmount = record.Amount;
            var recordType = record.Type;
            var recordCategoryId = record.CategoryId;

            var wallet = this.db.Wallets.FirstOrDefault(w => w.Id == 5);

            // Assert
            Assert.Equal("updated description", recordDescription);
            Assert.Equal(-20, recordAmount);
            Assert.Equal(RecordType.Expense, recordType);
            Assert.Equal(4, recordCategoryId);
            Assert.Equal(485, wallet.MoneyAmount);
        }

        [Fact]
        public async Task WalletAmountShouldIncreaseWith4WhenRecordUpdateMethodIsExecuted()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record1", 4, 5, "updated description", -5, 1, "Expense", DateTime.UtcNow);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record1");
            var recordDescription = record.Description;
            var recordAmount = record.Amount;
            var recordType = record.Type;
            var recordCategoryId = record.CategoryId;

            var wallet = this.db.Wallets.FirstOrDefault(w => w.Id == 5);

            // Assert
            Assert.Equal("updated description", recordDescription);
            Assert.Equal(-1, recordAmount);
            Assert.Equal(RecordType.Expense, recordType);
            Assert.Equal(4, recordCategoryId);
            Assert.Equal(504, wallet.MoneyAmount);
        }

        [Fact]
        public async Task WalletAmountShouldIncreaseWith10WhenChangeRecordType()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record1", 4, 5, "updated description", -5, 5, "Income", DateTime.UtcNow);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record1");
            var recordDescription = record.Description;
            var recordAmount = record.Amount;
            var recordType = record.Type;
            var recordCategoryId = record.CategoryId;

            var wallet = this.db.Wallets.FirstOrDefault(w => w.Id == 5);

            // Assert
            Assert.Equal("updated description", recordDescription);
            Assert.Equal(5, recordAmount);
            Assert.Equal(RecordType.Income, recordType);
            Assert.Equal(4, recordCategoryId);
            Assert.Equal(510, wallet.MoneyAmount);
        }

        [Fact]
        public async Task WalletAmountShouldIncreaseWith10WhenChangeRecordTypeAndTypeMinus5AsNewAmount()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record1", 4, 5, "updated description", -5, -5, "Income", DateTime.UtcNow);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record1");
            var recordDescription = record.Description;
            var recordAmount = record.Amount;
            var recordType = record.Type;
            var recordCategoryId = record.CategoryId;

            var wallet = this.db.Wallets.FirstOrDefault(w => w.Id == 5);

            // Assert
            Assert.Equal("updated description", recordDescription);
            Assert.Equal(5, recordAmount);
            Assert.Equal(RecordType.Income, recordType);
            Assert.Equal(4, recordCategoryId);
            Assert.Equal(510, wallet.MoneyAmount);
        }

        [Fact]
        public async Task WalletAmountShouldDecreaseWith30WhenChangeRecordType()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record3", 4, 5, "updated description", 15, 15, "Expense", DateTime.UtcNow);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record3");
            var recordDescription = record.Description;
            var recordAmount = record.Amount;
            var recordType = record.Type;
            var recordCategoryId = record.CategoryId;

            var wallet = this.db.Wallets.FirstOrDefault(w => w.Id == 5);

            // Assert
            Assert.Equal("updated description", recordDescription);
            Assert.Equal(-15, recordAmount);
            Assert.Equal(RecordType.Expense, recordType);
            Assert.Equal(4, recordCategoryId);
            Assert.Equal(470, wallet.MoneyAmount);
        }

        [Fact]
        public async Task WalletAmountShouldDecreaseWith10WhenChangeRecordAmount()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record3", 4, 5, "updated description", 15, 5, "Income", DateTime.UtcNow);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record3");
            var recordDescription = record.Description;
            var recordAmount = record.Amount;
            var recordType = record.Type;
            var recordCategoryId = record.CategoryId;

            var wallet = this.db.Wallets.FirstOrDefault(w => w.Id == 5);

            // Assert
            Assert.Equal("updated description", recordDescription);
            Assert.Equal(5, recordAmount);
            Assert.Equal(RecordType.Income, recordType);
            Assert.Equal(4, recordCategoryId);
            Assert.Equal(490, wallet.MoneyAmount);
        }

        [Fact]
        public async Task UpdateRecordShouldThrowsExceptionWhenRecordIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.recService.UpdateRecord("invalidRecordId", 4, 5, "updated description", 15, 5, "Income", DateTime.UtcNow));
        }

        [Fact]
        public async Task UpdateRecordShouldThrowsExceptionWhenWalletIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.recService.UpdateRecord("invalidRecordId", 4, 50, "updated description", 15, 5, "Income", DateTime.UtcNow));
        }

        [Fact]
        public async Task UpdateRecordShouldThrowsExceptionWhenCategoryIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.recService.UpdateRecord("record1", 40, 5, "updated description", 15, 5, "Income", DateTime.UtcNow));
        }

        [Fact]
        public async Task UpdateRecordShouldSetCreatedOnToTodayWhenInputDateIsDefault()
        {
            // Arrange
            this.FillDatabase();

            // Act
            string result = await this.recService.UpdateRecord("record3", 4, 5, "updated description", 15, 5, "Income", default);

            var record = this.db.Records.FirstOrDefault(r => r.Id == "record3");
            var recordDate = record.CreatedOn;

            // Assert
            Assert.Equal(DateTime.UtcNow.Day, recordDate.Day);

        }

        [Fact]
        public async Task UpdateRecordShouldThrowsExceptionWhenRecordTypeIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.recService.UpdateRecord("record1", 4, 5, "updated description", 15, 5, "invalidRecordType", DateTime.UtcNow));
        }

        [Fact]
        public async Task IsUserOwnRecordShouldReturnTrue()
        {
            // Arrange
            this.FillDatabase();

            // Act
            bool isUserOwnRecord = await this.recService.IsUserOwnRecordAsync("userId", "record1");

            Assert.True(isUserOwnRecord);
        }

        [Fact]
        public async Task IsUserOwnRecordShouldReturnFalse()
        {
            // Arrange
            this.FillDatabase();

            // Act
            bool isUserOwnRecord = await this.recService.IsUserOwnRecordAsync("userId", "invalidrecordId");

            Assert.False(isUserOwnRecord);
        }

        [Fact]
        public async Task EditWalletShouldThrowExceptionWhenWalletIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(() => this.recService.EditWalletAmountAsync(50, 200));
        }

        private void FillDatabase()
        {
            this.CleanDatabase();
            this.AddUser();
            this.AddCurrencies();
            this.AddWallets();
            this.AddCategories();
            this.AddRecords();
        }

        private void CleanDatabase()
        {
            foreach (var currency in this.db.Currencies)
            {
                this.db.Currencies.Remove(currency);
            }

            foreach (var record in this.db.Records)
            {
                this.db.Records.Remove(record);
            }

            foreach (var category in this.db.Categories)
            {
                this.db.Categories.Remove(category);
            }

            foreach (var wallet in this.db.Wallets)
            {
                this.db.Wallets.Remove(wallet);
            }

            foreach (var user in this.db.Users)
            {
                this.db.Users.Remove(user);
            }

            this.db.SaveChanges();
        }

        private IEnumerable<Currency> AddCurrencies()
        {
            var currencies = new List<Currency>();

            currencies.Add(new Currency
            {
                Id = 1,
                Code = "BGN",
                Name = "Bulgarian Lev",
            });

            currencies.Add(new Currency
            {
                Id = 2,
                Code = "USD",
                Name = "US Dollar",
            });

            currencies.Add(new Currency
            {
                Id = 3,
                Code = "EUR",
                Name = "Euro",
            });

            this.db.Currencies.AddRange(currencies);
            this.db.SaveChanges();

            return currencies;
        }

        private IEnumerable<Wallet> AddWallets()
        {
            var wallets = new List<Wallet>();

            wallets.Add(new Wallet
            {
                Id = 1,
                Name = "Home Wallet",
                Currency = new Currency
                {
                    Id = 4,
                    Code = "GBP",
                    Name = "Great Britain Pound",
                },
                MoneyAmount = 100,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
            });

            wallets.Add(new Wallet
            {
                Id = 2,
                Name = "Bussiness Wallet",
                Currency = new Currency
                {
                    Id = 5,
                    Code = "CHI",
                    Name = "China Iuan",
                },
                MoneyAmount = 200,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
            });

            wallets.Add(new Wallet
            {
                Id = 3,
                Name = "Job Wallet",
                Currency = new Currency
                {
                    Id = 6,
                    Code = "AUD",
                    Name = "Australian Dollar",
                },
                MoneyAmount = 300,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
            });

            this.db.Wallets.AddRange(wallets);
            this.db.SaveChanges();

            return wallets;
        }

        private IEnumerable<Category> AddCategories()
        {
            var wallet = new Wallet
            {
                Id = 4,
                Name = "Kids Wallet",
                Currency = new Currency
                {
                    Id = 7,
                    Code = "SRD",
                    Name = "Serbian Dinnar",
                },
                MoneyAmount = 400,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
            };

            this.db.Wallets.Add(wallet);
            this.db.SaveChanges();

            var categories = new List<Category>();

            categories.Add(new Category
            {
                Id = 1,
                Name = "Food",
                BadgeColor = BadgeColor.Success,
                Wallet = wallet,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WalletId = wallet.Id,
            });

            categories.Add(new Category
            {
                Id = 2,
                Name = "Alcohol",
                BadgeColor = BadgeColor.Danger,
                Wallet = wallet,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WalletId = wallet.Id,
            });

            categories.Add(new Category
            {
                Id = 3,
                Name = "Education",
                BadgeColor = BadgeColor.Info,
                Wallet = wallet,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WalletId = wallet.Id,
            });

            this.db.Categories.AddRange(categories);
            this.db.SaveChanges();

            return categories;
        }

        private IEnumerable<MoneySaver.Data.Models.Record> AddRecords()
        {
            var wallet = new Wallet
            {
                Id = 5,
                Name = "Holiday Wallet",
                Currency = new Currency
                {
                    Id = 8,
                    Code = "BRR",
                    Name = "Brazil Real",
                },
                MoneyAmount = 500,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
            };

            this.db.Wallets.Add(wallet);
            this.db.SaveChanges();

            var category = new Category
            {
                Id = 4,
                Name = "Parties",
                BadgeColor = BadgeColor.Warning,
                Wallet = wallet,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WalletId = wallet.Id,
            };

            this.db.Categories.Add(category);
            this.db.SaveChanges();

            var records = new List<MoneySaver.Data.Models.Record>()
            {
                new MoneySaver.Data.Models.Record()
                {
                    Id = "record1",
                    Amount = -5,
                    Category = category,
                    Type = RecordType.Expense,
                    Description = "Party in the club",
                    CategoryId = category.Id,
                    CreatedOn = DateTime.UtcNow,
                },

                new MoneySaver.Data.Models.Record()
                {
                    Id = "record2",
                    Amount = -10,
                    Category = category,
                    Type = RecordType.Expense,
                    Description = "Party in the hotel",
                    CategoryId = category.Id,
                    CreatedOn = DateTime.UtcNow,
                },

                new MoneySaver.Data.Models.Record()
                {
                    Id = "record3",
                    Amount = 15,
                    Category = category,
                    Type = RecordType.Income,
                    Description = "Bonus recieved",
                    CategoryId = category.Id,
                    CreatedOn = DateTime.UtcNow,
                },
            };

            this.db.Records.AddRange(records);
            this.db.SaveChanges();

            return records;
        }

        private ApplicationUser AddUser()
        {
            var user = new ApplicationUser()
            {
                Id = "userId",
                Email = "v.b.dimitrow@gmail.com",
                IsDeleted = false,
                PasswordHash = "UnhashedPass2021",
                UserName = "v.b.dimitrow@gmail.com",
                CreatedOn = DateTime.UtcNow,
                NormalizedUserName = "V.B.DIMITROW@GMAIL.COM",
                NormalizedEmail = "V.B.DIMITROW@GMAIL.COM",
                EmailConfirmed = false,
                SecurityStamp = "SecurityStamp",
                ConcurrencyStamp = "ConcurrencyStamp",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();

            return user;
        }
    }
}
