﻿namespace MoneySaver.Web.Tests.Controllers
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
    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Wallets;
    using Moq;
    using Xunit;

    public class WalletsControllerTests
    {
        private readonly IWalletsService walletsService;
        private readonly IRecordsService recordsService;
        private readonly ICurrenciesService currenciesService;
        private readonly ICategoriesService categoriesService;
        private readonly FakeUserManager userManager;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private WalletsController controller;
        private AddWalletInputModel inputModel;
        private EditWalletViewModel editModel;

        public WalletsControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("walletsDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.recordsService = new RecordsService(this.db);
            this.currenciesService = new CurrenciesService(this.db);
            this.categoriesService = new CategoriesService(this.db, this.recordsService);
            this.walletsService = new WalletsService(this.db, this.recordsService, this.currenciesService, this.categoriesService);
            this.userManager = new FakeUserManager();
            this.controller = new WalletsController(this.walletsService, this.recordsService, this.currenciesService, this.userManager);
        }

        [Fact]
        public async Task ControllerShouldCreateNewWallet()
        {
            this.FillDatabase();

            this.inputModel = new AddWalletInputModel
            {
                Amount = 250,
                CurrencyId = 1,
                Name = "Test Wallet",
            };

            await this.controller.Add(this.inputModel);

            var createdWallet = this.db.Wallets.FirstOrDefault(w => w.Name == "Test Wallet");

            Assert.Equal("Test Wallet", createdWallet.Name);
            Assert.Equal(1, createdWallet.CurrencyId);
            Assert.Equal(250, createdWallet.MoneyAmount);
        }

        [Fact]
        public async Task ControllerShouldReturnRedirectWhenWalletIsAdded()
        {
            this.FillDatabase();

            this.inputModel = new AddWalletInputModel
            {
                Amount = 250,
                CurrencyId = 1,
                Name = "Test Wallet",
            };

            var result = await this.controller.Add(this.inputModel);

            var redirectToActionResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("W", redirectToActionResult.Url[1].ToString());
        }

        [Fact]
        public async Task ControllerShouldReturnViewWithAllWallets()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.AllWallets();


            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<AllWalletsViewModel>>(viewResult.ViewData.Model);

            Assert.Equal(5, model.Count());
        }

        [Fact]
        public async Task ControllerShouldReturnRedirectToErrorWhenCurrencyIdIsInvalid()
        {
            this.FillDatabase();

            this.inputModel = new AddWalletInputModel
            {
                Amount = 250,
                CurrencyId = 100,
                Name = "Test Wallet",
            };

            var result = await this.controller.Add(this.inputModel);

            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("H", redirectResult.Url[1].ToString());
        }

        [Fact]
        public async Task EditWalletShouldReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Edit(5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditWalletViewModel>(viewResult.ViewData.Model);

            Assert.Equal(5, model.Id);
            Assert.Equal("Holiday Wallet", model.Name);
            Assert.Equal(8, model.CurrencyId);
            Assert.Equal("BRR", model.CurrentCurrencyCode);
            Assert.Equal("Brazil Real", model.CurrentCurrencyName);
            Assert.Equal(500, model.Amount);
        }

        [Fact]
        public async Task EditShouldEditWalletSuccessfully()
        {
            // Arrange
            this.FillDatabase();
            this.editModel = new EditWalletViewModel
            {
                Id = 1,
                Amount = 560,
                Name = "Test Edit Wallet",
                CurrencyId = 5,
            };

            // Act
            var result = await this.controller.Edit(this.editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var editedWallet = this.db.Wallets.Find(1);

            Assert.Equal(1, editedWallet.Id);
            Assert.Equal("Test Edit Wallet", editedWallet.Name);
            Assert.Equal(5, editedWallet.CurrencyId);
            Assert.Equal(560, editedWallet.MoneyAmount);
        }

        [Fact]
        public async Task DeleteShouldEditWalletSuccessfully()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var deletedWallet = this.db.Wallets.Find(1);

            Assert.Null(deletedWallet);
            Assert.Equal(4, this.db.Wallets.Count());
        }

        [Fact]
        public async Task RecordsShouldReturnViewWithAllRecordsInWallet5()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Records(5, 1);


            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
        }

        [Fact]
        public async Task RecordsShouldReturnViewWith0RecordsInWallet1()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Records(1, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(0, model.Records.Count());
        }

        [Fact]
        public async Task WalletDetailsShouldReturnViewWithCorrectView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Details(5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletDetailsViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
            Assert.Equal(2, model.Categories.Count());
            Assert.Equal("BRR", model.Currency);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("Holiday Wallet", model.WalletName);
            Assert.Equal(-15, model.TotalWalletExpenses);
            Assert.Equal(15, model.TotalWalletIncomes);
            Assert.Equal(-15, model.TotalWalletExpensesLast30Days);
            Assert.Equal(15, model.TotalWalletIncomesLast30Days);
            Assert.Equal(500, model.CurrentBalance);
        }

        [Fact]
        public async Task SearchShouldReturn2ResultsWhenKeywordIsParty()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Search(5, "party", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(2, model.Records.Count());
            Assert.Equal(2, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
        }

        [Fact]
        public async Task SearchShouldReturn3ResultsWhenKeywordIsNull()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Search(5, null, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
            Assert.Equal(3, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
        }

        [Fact]
        public async Task SearchShouldReturn3ResultsWhenKeywordIsEmpty()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Search(5, string.Empty, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
            Assert.Equal(3, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
        }

        [Fact]
        public async Task SearchShouldReturn3ResultsWhenDateRangeIsBetweenTodayAndYesterday()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.DateSorted(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, 5, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
            Assert.Equal(3, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
        }

        [Fact]
        public async Task SearchShouldReturn0ResultsWhenDateRangeIsAfter2And3Days()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.DateSorted(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 5, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<WalletSearchResultViewModel>(viewResult.ViewData.Model);

            Assert.Equal(0, model.Records.Count());
            Assert.Equal(0, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
        }

        [Fact]
        public async Task CategoriesShouldReturn2Categories()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Categories(5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StatisticsWalletViewModel>(viewResult.ViewData.Model);

            Assert.Equal(2, model.Categories.Count());
            Assert.Equal("Holiday Wallet", model.WalletName);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("BRR", model.Currency);
            Assert.Equal(15, model.Incomes);
            Assert.Equal(-15, model.Outcomes);
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