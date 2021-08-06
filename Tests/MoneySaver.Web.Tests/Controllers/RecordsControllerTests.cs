namespace MoneySaver.Web.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Controllers;
    using MoneySaver.Web.ViewModels.Records;
    using Moq;
    using Xunit;

    public class RecordsControllerTests
    {
        private readonly IWalletsService walletsService;
        private readonly IRecordsService recordsService;
        private readonly ICurrenciesService currenciesService;
        private readonly ICategoriesService categoriesService;
        private readonly FakeUserManager userManager;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private RecordsController controller;
        private AddRecordViewModel inputModel;

        public RecordsControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("recordsDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.recordsService = new RecordsService(this.db);
            this.currenciesService = new CurrenciesService(this.db);
            this.categoriesService = new CategoriesService(this.db, this.recordsService);
            this.walletsService = new WalletsService(this.db, this.recordsService, this.currenciesService, this.categoriesService);
            this.userManager = new FakeUserManager();
            this.controller = new RecordsController(this.recordsService, this.walletsService, this.userManager);
        }

        [Fact]
        public async Task AddGetShoudReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Add(3);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddRecordViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Job Wallet", model.WalletName);
        }

        public async Task AddPostShoudCreateRecordSuccessfully()
        {
            // Arrange
            this.FillDatabase();

            this.inputModel = new AddRecordViewModel
            {
                Amount = 20,
                WalletId = 5,
                CategoryId = 4,
                Type = ViewModels.Records.Enums.RecordTypeInputModel.Expense,
                Description = "Test Record",
                CreatedOn = DateTime.UtcNow,
            };

            // Act
            var result = await this.controller.Add(this.inputModel);

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);
            var record = this.db.Records.FirstOrDefault(x => x.Description == "Test Record");

            Assert.Equal("Test Record", record.Description);
            Assert.Equal(4, record.CategoryId);
            Assert.True(record.Type == RecordType.Expense);
            Assert.Equal(-20, record.Amount);
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
