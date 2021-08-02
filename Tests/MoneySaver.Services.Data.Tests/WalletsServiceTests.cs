namespace MoneySaver.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Data;
    using MoneySaver.Data.Common.Repositories;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Wallets;
    using Moq;
    using Xunit;

    public class WalletsServiceTests
    {
        private Mock<IRecordsService> recService;
        private Mock<ICategoriesService> catService;
        private Mock<ICurrenciesService> currService;
        private IWalletsService wallService;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;

        public WalletsServiceTests()
        {
            this.recService = new Mock<IRecordsService>();
            this.catService = new Mock<ICategoriesService>();
            this.currService = new Mock<ICurrenciesService>();
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("database");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.wallService = new WalletsService(this.db, this.recService.Object, this.currService.Object, this.catService.Object);
        }

        [Fact]
        public async Task GetWalletWithCategoriesAsyncShoudReturnValidDto()
        {
            this.FillDatabase();
            var firstWallet = this.db.Wallets
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            // Act
            var result = await this.wallService.GetWalletWithCategoriesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(firstWallet.Name, result.WalletName);
            Assert.Equal(firstWallet.MoneyAmount, result.TotalAmount);
        }

        [Fact]
        public void WalletCountShouldBe4WhenRemoveIsExecuted()
        {
            this.FillDatabase();
            var firstWallet = this.db.Wallets
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            // Act
            var result = this.wallService.RemoveAsync(2);

            // Assert
            Assert.Equal(4, this.db.Wallets.Count());
        }

        [Fact]
        public void MethodShouldThrowExceptionWhenWalletIdIsInvalid()
        {
            this.FillDatabase();

            // Act
            var result = this.wallService.RemoveAsync(10);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => this.wallService.GetWalletWithCategoriesAsync(10));
        }

        [Fact]
        public void CategoriesCountShouldBe0WhenRemoveWallet()
        {
            // Arrange
            this.FillDatabase();
            var firstWallet = this.db.Wallets
               .FirstOrDefault(x => x.Id == 4);

            // Act
            var result = this.wallService.RemoveAsync(4);

            // Assert
            Assert.Equal(0, firstWallet.Categories.Count());
        }

        [Fact]
        public void WalletsCountShouldBe6WhenAddWallet()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = this.wallService.AddAsync("userId", "Test Wallet", 1200, 1);

            // Assert
            Assert.Equal(6, this.db.Wallets.Count());
        }

        [Fact]
        public void MethodShouldThrowExceptionWhenWalletWithThisNameAlreadyExistInAccount()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => this.wallService.AddAsync("userId", "Home Wallet", 1000, 1));
        }

        [Fact]
        public void MethodShouldThrowExceptionWhenCurrencyDoesntExist()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => this.wallService.AddAsync("userId", "Test Wallet", 1000, 10));
        }

        [Fact]
        public void WalletDataShouldBeChangedWhenMethodIsExecuted()
        {
            // Arrange
            this.FillDatabase();

            var wallet = new Wallet()
            {
                Id = 1,
                MoneyAmount = 2000,
                Name = "New Wallet",
                CurrencyId = 5,
            };

            // Act
            this.wallService
                .EditAsync("userId",  wallet.Id, wallet.Name, wallet.MoneyAmount, wallet.CurrencyId);

            var changedWallet = this.db.Wallets.Find(wallet.Id);

            // Assert
            Assert.Equal(wallet.Id, changedWallet.Id);
            Assert.Equal(wallet.MoneyAmount, changedWallet.MoneyAmount);
            Assert.Equal(wallet.Name, changedWallet.Name);
            Assert.Equal(wallet.CurrencyId, changedWallet.CurrencyId);

        }

        [Fact]
        public async Task WalletDataShouldReturnWalletInfoCorrectly()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.wallService
                .GetWalletInfoForEditAsync("userId", 1);

            // Assert
            Assert.Equal("Home Wallet", result.Name);
            Assert.Equal("GBP", result.CurrentCurrencyCode);
            Assert.Equal(4, result.CurrencyId);
            Assert.Equal(100, result.Amount);
        }

        [Fact]
        public async Task WalletsCountShouldBe5()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.wallService
                .GetAllWalletsAsync("userId");

            var firstWallet = result.FirstOrDefault(x => x.WalletId == 1);

            // Assert
            Assert.Equal(5, result.Count());
            Assert.Equal("Home Wallet", firstWallet.WalletName);
            Assert.Equal("GBP", firstWallet.Currency);
            Assert.Equal(100, firstWallet.CurrentBalance);
        }

        [Fact]
        public async Task WalletNameShoudBeHomeWallet()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.wallService
                .GetWalletNameAsync(1);

            // Assert
            Assert.Equal("Home Wallet", result);
        }

        [Fact]
        public async Task MethodShouldReturnExceptionWhenWalletIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.wallService.GetWalletNameAsync(10));
        }

        [Fact]
        public async Task MethodShouldReturnWalletInfoCorrectly()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.wallService
                .GetWalletDetailsAsync("userId", 1);

            // Assert
            Assert.Equal("Home Wallet", result.WalletName);
            Assert.Equal(100, result.CurrentBalance);
        }

        [Fact]
        public async Task MethodShouldReturnExceptionWhenWalletIdIsNotExist()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.wallService
                .GetWalletDetailsAsync("userId", 10));
        }

        [Fact]
        public async Task WalletIdShouldBe5()
        {
            // Arrange
            this.FillDatabase();

            var walletId = await this.wallService
                .GetWalletIdByRecordIdAsync("record1");

            // Assert
            Assert.Equal(5, walletId);
        }

        [Fact]
        public async Task MethodShouldReturnExceptionWhenRecordIdIsNotExist()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.wallService
                .GetWalletIdByRecordIdAsync("unexistingRecordId"));
        }

        [Fact]
        public async Task CategoriesCountShouldBe3()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var categories = await this.wallService
                .GetWalletCategoriesAsync(4);

            // Assert
            Assert.Equal(3, categories.Count());
        }

        [Fact]
        public void GetCountShouldReturn3()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var recordsCount = this.wallService
                .GetCount(5);

            // Assert
            Assert.Equal(3, recordsCount);
        }

        [Fact]
        public async Task RecordsCountShouldBe3()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetAllRecordsAsync(1, 5, 12);

            // Assert
            Assert.Equal(3, records.Count());
        }

        [Fact]
        public async Task RecordDescriptionShouldBeBonusRecieved()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetAllRecordsAsync(1, 5, 12);
            var lastRecord = records.FirstOrDefault();

            // Assert
            Assert.Equal("Bonus recieved", lastRecord.Description);
        }

        [Fact]
        public async Task GetAllRecordsShouldReturnExceptionWhenWalletIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.wallService
                .GetAllRecordsAsync(1, 50, 12));
        }

        [Fact]
        public void GetSearchRecordsCountShouldReturn2()
        {
            // Arrange
            this.FillDatabase();

            // Act
            int records = this.wallService
                .GetSearchRecordsCount("party", 5);

            // Assert
            Assert.Equal(2, records);
        }

        [Fact]
        public void GetSearchRecordsCountShouldReturn3WhenSearchTermIsEmptyString()
        {
            // Arrange
            this.FillDatabase();

            // Act
            int records = this.wallService
                .GetSearchRecordsCount(string.Empty, 5);

            // Assert
            Assert.Equal(3, records);
        }

        [Fact]
        public void GetSearchRecordsCountShouldReturn3WhenSearchTermIsNull()
        {
            // Arrange
            this.FillDatabase();

            // Act
            int records = this.wallService
                .GetSearchRecordsCount(null, 5);

            // Assert
            Assert.Equal(3, records);
        }

        [Fact]
        public async Task GetRecordsByKeyWordShouldReturn3RecordsWhenTermIsEmpty()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetRecordsByKeywordAsync(string.Empty, 5, 1, 12);

            // Assert
            Assert.Equal(3, records.Count());
        }

        [Fact]
        public async Task GetRecordsByKeyWordShouldReturn2RecordsWhenTermIsParty()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetRecordsByKeywordAsync("party", 5, 1, 12);

            // Assert
            Assert.Equal(2, records.Count());
        }

        [Fact]
        public async Task GetRecordsByKeyWordShouldReturn1RecordsWhenTermIsBonus()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetRecordsByKeywordAsync("bonus", 5, 1, 12);

            // Assert
            Assert.Equal(1, records.Count());
        }

        [Fact]
        public async Task RecordDescriptionShouldBeBonusRecievedWhenSearchTermIsBonus()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetRecordsByKeywordAsync("bonus", 5, 1, 12);

            var record = records.FirstOrDefault();

            // Assert
            Assert.Equal("Bonus recieved", record.Description);
        }

        [Fact]
        public async Task GetDateSortedShouldReturn3Records()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var records = await this.wallService
                .GetRecordsByDateRangeAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, 5, 1, 12);

            // Assert
            Assert.Equal(3, records.Count());
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
                    Amount = 5,
                    Category = category,
                    Type = RecordType.Expense,
                    Description = "Party in the club",
                    CategoryId = category.Id,
                    CreatedOn = DateTime.UtcNow,
                },

                new MoneySaver.Data.Models.Record()
                {
                    Id = "record2",
                    Amount = 10,
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
