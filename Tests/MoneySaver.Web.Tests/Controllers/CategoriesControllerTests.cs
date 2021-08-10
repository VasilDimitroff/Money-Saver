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
    using MoneySaver.Web.ViewModels.Categories;
    using Moq;
    using Xunit;

    public class CategoriesControllerTests
    {
        private readonly IWalletsService walletsService;
        private readonly IRecordsService recordsService;
        private readonly ICategoriesService categoriesService;
        private readonly ICurrenciesService currenciesService;
        private readonly FakeUserManager userManager;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private CategoriesController controller;
        private AddCategoryInputModel inputModel;
        private EditCategoryInputModel editModel;
        private DeleteCategoryInputModel deleteModel;

        public CategoriesControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("categoriesDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.recordsService = new RecordsService(this.db);
            this.currenciesService = new CurrenciesService(this.db);
            this.categoriesService = new CategoriesService(this.db, this.recordsService);
            this.walletsService = new WalletsService(this.db, this.recordsService, this.currenciesService, this.categoriesService);
            this.userManager = new FakeUserManager();
            this.controller = new CategoriesController(this.categoriesService, this.walletsService, this.userManager);
        }

        [Fact]
        public async Task AddGetShoudReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Add(5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddCategoryInputModel>(viewResult.ViewData.Model);
            Assert.Equal("Holiday Wallet", model.WalletName);
        }

        [Fact]
        public async Task AddPostShoudCreateCategoryAndReturnRedirect()
        {
            // Arrange
            this.FillDatabase();

            this.inputModel = new AddCategoryInputModel
            {
                WalletId = 5,
                Name = "Test Category",
                BadgeColor = "Danger",
            };

            // Act
            var result = await this.controller.Add(this.inputModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);

            var category = this.db.Categories.FirstOrDefault(c => c.Name.Contains("Test Category"));
            Assert.NotNull(category);
            Assert.Equal("Test Category", category.Name);
            Assert.Equal(5, category.WalletId);
            Assert.Equal(3, this.db.Wallets.FirstOrDefault(w => w.Id == 5).Categories.Count());
        }

        [Fact]
        public async Task EditGetShoudReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Edit(4, 5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditCategoryInputModel>(viewResult.ViewData.Model);

            Assert.Equal("Holiday Wallet", model.WalletName);
            Assert.Equal("Parties", model.CategoryName);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.BadgeColor);
        }

        [Fact]
        public async Task EditPostShoudEditCategorySuccessfully()
        {
            // Arrange
            this.FillDatabase();

            this.editModel = new EditCategoryInputModel
            {
                CategoryId = 4,
                CategoryName = "Edited Category Name",
                BadgeColor = ViewModels.Records.Enums.BadgeColor.Danger,
            };

            // Act
            var result = await this.controller.Edit(this.editModel);

            // Assert
            var editedCategory = this.db.Categories.FirstOrDefault(r => r.Id == 4);

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("Edited Category Name", editedCategory.Name);
            Assert.Equal(4, editedCategory.Id);
            Assert.Equal(5, editedCategory.WalletId);
            Assert.Equal(BadgeColor.Danger, editedCategory.BadgeColor);
        }

        [Fact]
        public async Task DeleteGetShoudReturnView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Delete(4, 5);

            // Assert

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DeleteCategoryInputModel>(viewResult.ViewData.Model);
            Assert.Equal("Parties", model.OldCategoryName);
            Assert.Equal(4, model.OldCategoryId);
            Assert.Equal(5, model.WalletId);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.OldCategoryBadgeColor);
        }

        [Fact]
        public async Task DeleteShouldDeleteCategoryAndReturnRedirect()
        {
            // Arrange
            this.FillDatabase();
            this.deleteModel = new DeleteCategoryInputModel
            {
                OldCategoryId = 4,
                OldCategoryName = "Parties",
                WalletId = 5,
                WalletName = "Holiday Wallet",
                NewCategoryId = -1,
                OldCategoryBadgeColor = ViewModels.Records.Enums.BadgeColor.Warning,
            };

            // Act
            var result = await this.controller.Delete(this.deleteModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("W", redirectResult.Url[1].ToString());

            var category = this.db.Categories.FirstOrDefault(c => c.Id == 4);
            Assert.Null(category);
            Assert.Equal(1, this.db.Wallets.FirstOrDefault(w => w.Id == 5).Categories.Count());
        }

        [Fact]
        public async Task DetailsShoudReturnView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Details(4, 1);

            // Assert

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryRecordsViewModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Records.Count());
            Assert.Equal(3, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("Parties", model.Category);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.BadgeColor);
            Assert.Equal("BRR", model.Currency);
        }

        [Fact]
        public async Task SearchShouldReturn2Results()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Search(4, "party", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryRecordsViewModel>(viewResult.ViewData.Model);

            Assert.Equal(2, model.Records.Count());
            Assert.Equal(2, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("Parties", model.Category);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.BadgeColor);
            Assert.Equal("BRR", model.Currency);
        }

        [Fact]
        public async Task SearchShouldReturn3ResultsWhenSearchTermIsNull()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Search(4, null, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryRecordsViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
            Assert.Equal(3, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("Parties", model.Category);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.BadgeColor);
            Assert.Equal("BRR", model.Currency);
        }

        [Fact]
        public async Task SearchShouldReturn3ResultsWhenDateRangeIsBetweenTodayAndYesterday()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.DateSorted(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, 4, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryRecordsViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Records.Count());
            Assert.Equal(3, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("Parties", model.Category);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.BadgeColor);
            Assert.Equal("BRR", model.Currency);
        }

        [Fact]
        public async Task SearchShouldReturn0ResultsWhenDateRangeIsAfter2And3Days()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.DateSorted(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 4, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryRecordsViewModel>(viewResult.ViewData.Model);

            Assert.Equal(0, model.Records.Count());
            Assert.Equal(0, model.RecordsCount);
            Assert.Equal(5, model.WalletId);
            Assert.Equal("Parties", model.Category);
            Assert.Equal(ViewModels.Records.Enums.BadgeColor.Warning, model.BadgeColor);
            Assert.Equal("BRR", model.Currency);
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
