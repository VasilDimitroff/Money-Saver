namespace MoneySaver.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Categories;
    using Xunit;

    public class CategoriesServiceTests
    {
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private IRecordsService recService;
        private ICategoriesService catService;

        public CategoriesServiceTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("categoriesDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.recService = new RecordsService(this.db);
            this.catService = new CategoriesService(this.db, this.recService);
        }

        [Fact]
        public async Task AddCategoryShoudCreateValidObject()
        {
            // Arrange
            this.FillDatabase();

            // Act
            await this.catService.AddAsync("Test Category", 5, "Success");

            var category = this.db.Categories.FirstOrDefault(x => x.Name == "Test Category");
            var wallet = this.db.Wallets.FirstOrDefault(x => x.Id == category.WalletId);

            Assert.Equal("Test Category", category.Name);
            Assert.Equal(BadgeColor.Success, category.BadgeColor);
            Assert.Equal(5, category.WalletId);
            Assert.Equal(3, wallet.Categories.Count());
        }

        [Fact]
        public async Task AddCategoryShoudThrowExceptionWhenWalletIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.AddAsync("Test Category", 50, "Success"));
        }

        [Fact]
        public async Task AddCategoryShoudThrowExceptionWhenWalletIdIsNull()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.AddAsync(null, 5, "Success"));
        }

        [Fact]
        public async Task AddCategoryShoudThrowExceptionWhenWalletIdIsEmptyString()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.AddAsync(string.Empty, 5, "Success"));
        }

        [Fact]
        public async Task AddCategoryShoudThrowExceptionWhenCategoryAlreadyExist()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.AddAsync("parties", 5, "Success"));
        }

        [Fact]
        public async Task AddCategoryShoudThrowExceptionWhenBadgeColorIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.AddAsync("Test Category", 5, "InvalidBadgeColor"));
        }

        [Fact]
        public async Task CategoryShoudRemoveCategoryWithAllRecordsWithin()
        {
            // Arrange
            this.FillDatabase();

            // Act
            await this.catService.RemoveAsync(4, -1);

            var category = this.db.Categories.FirstOrDefault(x => x.Id == 4);
            var wallet = this.db.Wallets.FirstOrDefault(x => x.Id == 5);

            Assert.Null(category);
            Assert.Equal(1, wallet.Categories.Count());
        }

        [Fact]
        public async Task RemoveCategoryShoudMoveRecordsToCategoryWithId5()
        {
            // Arrange
            this.FillDatabase();

            // Act
            await this.catService.RemoveAsync(4, 5);

            var oldCategory = this.db.Categories.FirstOrDefault(x => x.Id == 4);
            var newCategory = this.db.Categories.FirstOrDefault(x => x.Id == 5);
            var wallet = this.db.Wallets.FirstOrDefault(x => x.Id == 5);

            Assert.Null(oldCategory);
            Assert.Equal(1, wallet.Categories.Count());
            Assert.Equal(3, newCategory.Records.Count());
        }

        [Fact]
        public async Task RemoveCategoryShoudThrowExceptionWhenOldCategoryIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.catService.RemoveAsync(50, 5));
        }

        [Fact]
        public async Task RemoveCategoryShoudThrowExceptionWhenNewCategoryIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.catService.RemoveAsync(4, 50));
        }

        [Fact]
        public async Task EditCategoryShouldEditObjectSuccessfully()
        {
            // Arrange
            this.FillDatabase();

            // Act
            await this.catService.EditAsync(4, "Edited category", "Dark");

            var category = this.db.Categories.FirstOrDefault(x => x.Id == 4);

            Assert.Equal("Edited category", category.Name);
            Assert.Equal(BadgeColor.Dark, category.BadgeColor);
        }

        [Fact]
        public async Task EditCategoryShouldThrowExceptionWhenCategoryIdIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.catService.EditAsync(40, "Edited category", "Dark"));
        }

        [Fact]
        public async Task EditCategoryShouldThrowExceptionWhenBadgeColorIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.EditAsync(4, "Edited category", "InvalidBadgeColor"));
        }

        [Fact]
        public async Task GetRecordsShouldThrowExceptionWhenCategoryIsInvalid()
        {
            // Arrange
            this.FillDatabase();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => this.catService.GetRecordsByKeywordAsync("party", 40, 1, 12));
        }

        [Fact]
        public async Task GetRecordsByKeywordShouldReturn2ValidObjectsWithSearchTerm()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.catService.GetRecordsByKeywordAsync("party", 4, 1, 12);

            var firstRecord = result.Records.FirstOrDefault(x => x.Id == "record1");

            Assert.Equal(2, result.Records.Count());
            Assert.Equal("Party in the club", firstRecord.Description);
        }

        [Fact]
        public async Task GetRecordsByKeywordShouldReturnAllRecordsInWalletWhenSearchTermIsNull()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.catService.GetRecordsByKeywordAsync(null, 4, 1, 12);

            var firstRecord = result.Records.FirstOrDefault(x => x.Id == "record1");

            Assert.Equal(3, result.Records.Count());
            Assert.Equal("Party in the club", firstRecord.Description);
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

            var category2 = new Category
            {
                Id = 5,
                Name = "Tickets",
                BadgeColor = BadgeColor.Info,
                Wallet = wallet,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WalletId = wallet.Id,
            };

            this.db.Categories.Add(category2);
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
