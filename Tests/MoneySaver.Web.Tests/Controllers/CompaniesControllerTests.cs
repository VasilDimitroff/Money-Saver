namespace MoneySaver.Web.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Common.Repositories;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Data.Repositories;
    using MoneySaver.Services.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Areas.Administration.Controllers;
    using MoneySaver.Web.ViewModels.Companies;
    using Moq;
    using Xunit;

    public class CompaniesControllerTests
    {
        private readonly ITradesService tradesService;
        private readonly ICompaniesService companiesService;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private IDeletableEntityRepository<Company> repo;
        private CompaniesController controller;
        private AddCompanyInputModel inputModel;
        private EditCompanyInputModel editModel;
        private TempDataDictionary tempData;
        private DefaultHttpContext httpContext;

        public CompaniesControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("companiesDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.repo = new EfDeletableEntityRepository<Company>(this.db);
            this.companiesService = new CompaniesService(this.repo);
            this.tradesService = new TradesService(this.db, this.companiesService);
            this.httpContext = new DefaultHttpContext();
            this.tempData = new TempDataDictionary(this.httpContext, Mock.Of<ITempDataProvider>());
            this.controller = new CompaniesController(this.companiesService);
            this.controller.TempData = this.tempData;
        }

        [Fact]
        public void AddShouldReturnView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = this.controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddShouldCreateCompanyAndReturnViewResultWhenUserIsAdmin()
        {
            // Arrange
            this.FillDatabase();
            this.inputModel = new AddCompanyInputModel
            {
                CompanyName = "Test Company",
                Ticker = "TCO",
            };

            this.controller.TempData["InvalidAddModel"] = "Some of fields you entered are invalid!";
            this.controller.TempData["CompanyExist"] = $"Company with ticker {this.inputModel.Ticker} already exists!";
            this.controller.TempData["SuccessfullAddedCompany"] =
                       $"Successfilly added company with name {this.inputModel.CompanyName} and ticker {this.inputModel.Ticker}!";

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name"),
                    new Claim(ClaimTypes.NameIdentifier, "userId"),
                    new Claim(ClaimTypes.Role, GlobalConstants.AdministratorRoleName),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"));

            this.controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            // Act
            var result = await this.controller.Add(this.inputModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var createdCompany = this.db.Companies.FirstOrDefault(x => x.Name == "Test Company");

            Assert.Equal("TCO", createdCompany.Ticker);
            Assert.Equal("Test Company", createdCompany.Name);
            Assert.Equal(4, this.db.Companies.Count());
        }

        [Fact]
        public async Task AddShouldCreateCompanyAndReturnRedirectResultWhenUserIsNotAdmin()
        {
            // Arrange
            this.FillDatabase();
            this.inputModel = new AddCompanyInputModel
            {
                CompanyName = "Test Company",
                Ticker = "TCO",
            };

            this.controller.TempData["InvalidAddModel"] = "Some of fields you entered are invalid!";
            this.controller.TempData["CompanyExist"] = $"Company with ticker {this.inputModel.Ticker} already exists!";
            this.controller.TempData["SuccessfullAddedCompany"] =
                       $"Successfilly added company with name {this.inputModel.CompanyName} and ticker {this.inputModel.Ticker}!";

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name"),
                    new Claim(ClaimTypes.NameIdentifier, "userId"),
                    new Claim(ClaimTypes.Role, "nonAdminRole"),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"));

            this.controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            // Act
            var result = await this.controller.Add(this.inputModel);

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);
            var createdCompany = this.db.Companies.FirstOrDefault(x => x.Name == "Test Company");

            Assert.Equal("TCO", createdCompany.Ticker);
            Assert.Equal("Test Company", createdCompany.Name);
            Assert.Equal(4, this.db.Companies.Count());
        }

        [Fact]
        public async Task EditShouldReturnView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Edit("company1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditCompanyInputModel>(viewResult.ViewData.Model);
            Assert.Equal("company1", model.Id);
            Assert.Equal("Tesla", model.Name);
            Assert.Equal("TSLA", model.Ticker);
        }

        [Fact]
        public async Task EditPostShouldEditCompanySuccessfully()
        {
            // Arrange
            this.FillDatabase();
            this.editModel = new EditCompanyInputModel
            {
                Id = "company1",
                Name = "New Name",
                Ticker = "TsLNn",
            };

            // Act
            var result = await this.controller.Edit(this.editModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CompanyViewModel>>(viewResult.ViewData.Model);

            var editedCompany = this.db.Companies.FirstOrDefault(x => x.Id == "company1");

            Assert.Equal("company1", editedCompany.Id);
            Assert.Equal("New Name", editedCompany.Name);
            Assert.Equal("TSLNN", editedCompany.Ticker);
        }
        
        [Fact]
        public async Task DeleteShouldReturnView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Delete("company1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CompanyViewModel>(viewResult.ViewData.Model);
            Assert.Equal("company1", model.Id);
            Assert.Equal("Tesla", model.Name);
            Assert.Equal("TSLA", model.Ticker);
        }

        /*
        [Fact]
        public async Task DeleteConfirmedShouldSetCompanyAsDeleted()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.DeleteConfirmed("company2");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CompanyViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, this.repo.All().Count());
            Assert.Equal(1, this.db.Trades.Where(t => !t.Company.IsDeleted).Count());
        }
        */

        [Fact]
        public async Task RestoreShouldUndeleteCompany()
        {
           // Arrange
           this.FillDatabase();
           this.controller.TempData["SuccessfullRestoredCompany"] = $"Successfully restored company Facebook!";

           // Act
           await this.controller.DeleteConfirmed("company2");
           Assert.Equal(2, this.repo.All().Count());
           Assert.Equal(1, this.db.Trades.Where(t => !t.Company.IsDeleted).Count());

           var result = await this.controller.Restore("company2");

           // Assert
           var viewResult = Assert.IsType<ViewResult>(result);
           var model = Assert.IsAssignableFrom<List<CompanyViewModel>>(viewResult.ViewData.Model);
           Assert.Equal(3, this.repo.All().Count());
           Assert.Equal(3, this.db.Trades.Where(t => !t.Company.IsDeleted).Count());
        }

        private void FillDatabase()
        {
            this.CleanDatabase();
            this.AddUser();
            this.AddAdminRole();
            this.MakeUserAdmin();
            this.AddCurrencies();
            this.AddCompanies();
            this.AddInvestmentWallets();
            this.AddTrades();
        }

        private void CleanDatabase()
        {
            foreach (var trade in this.db.Trades)
            {
                this.db.Trades.Remove(trade);
                this.db.SaveChanges();
            }

            foreach (var wallet in this.db.InvestmentWallets)
            {
                this.db.InvestmentWallets.Remove(wallet);
                this.db.SaveChanges();
            }

            foreach (var company in this.db.Companies)
            {
                this.db.Companies.Remove(company);
                this.db.SaveChanges();
            }

            foreach (var currency in this.db.Currencies)
            {
                this.db.Currencies.Remove(currency);
                this.db.SaveChanges();
            }

            foreach (var user in this.db.Users)
            {
                this.db.Users.Remove(user);
                this.db.SaveChanges();
            }

            foreach (var role in this.db.UserRoles)
            {
                this.db.UserRoles.Remove(role);
                this.db.SaveChanges();
            }

            foreach (var role in this.db.Roles)
            {
                this.db.Roles.Remove(role);
                this.db.SaveChanges();
            }

            this.db.SaveChanges();
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

        private IEnumerable<Company> AddCompanies()
        {
            var companies = new List<Company>();

            companies.Add(new Company
            {
                Id = "company1",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Tesla",
                Ticker = "TSLA",
                Trades = new HashSet<Trade>(),
            });

            companies.Add(new Company
            {
                Id = "company2",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Facebook",
                Ticker = "FB",
                Trades = new HashSet<Trade>(),
            });

            companies.Add(new Company
            {
                Id = "company3",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Amazon",
                Ticker = "AMZN",
                Trades = new HashSet<Trade>(),
            });

            this.db.Companies.AddRange(companies);
            this.db.SaveChanges();

            return companies;
        }

        private void AddInvestmentWallets()
        {
            var wallets = new List<InvestmentWallet>();

            wallets.Add(new InvestmentWallet
            {
                Id = 1,
                CurrencyId = 1,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
                Name = "BGN Account",
                Trades = new HashSet<Trade>(),
            });

            wallets.Add(new InvestmentWallet
            {
                Id = 2,
                CurrencyId = 2,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
                Name = "USD Account",
                Trades = new HashSet<Trade>(),
            });

            wallets.Add(new InvestmentWallet
            {
                Id = 3,
                CurrencyId = 3,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
                Name = "EUR Account",
                Trades = new HashSet<Trade>(),
            });

            this.db.InvestmentWallets.AddRange(wallets);
            this.db.SaveChanges();
        }

        private IEnumerable<Trade> AddTrades()
        {
            var trades = new List<Trade>();

            trades.Add(new Trade
            {
                Id = "trade1",
                CompanyId = "company1",
                InvestmentWalletId = 1,
                Price = -100,
                StockQuantity = 5,
                CreatedOn = DateTime.UtcNow,
                Type = TradeType.Buy,
            });

            trades.Add(new Trade
            {
                Id = "trade2",
                CompanyId = "company2",
                InvestmentWalletId = 2,
                Price = -200,
                StockQuantity = 10,
                CreatedOn = DateTime.UtcNow,
                Type = TradeType.Buy,
            });

            trades.Add(new Trade
            {
                Id = "trade3",
                CompanyId = "company2",
                InvestmentWalletId = 2,
                Price = 50,
                StockQuantity = 2,
                CreatedOn = DateTime.UtcNow,
                Type = TradeType.Sell,
            });

            this.db.Trades.AddRange(trades);
            this.db.SaveChanges();

            return trades;
        }

        private void AddAdminRole()
        {
            var appRole = new ApplicationRole
            {
                Name = GlobalConstants.AdministratorRoleName,
                Id = "adminRole",
            };

            this.db.Roles.Add(appRole);
            this.db.SaveChanges();
        }

        private void MakeUserAdmin()
        {
            var user = this.db.Users
                 .FirstOrDefault(u => u.UserName == "v.b.dimitrow@gmail.com");

            var userRole = new IdentityUserRole<string>()
            {
                RoleId = "adminRole",
                UserId = "userId",
            };

            user.Roles.Add(userRole);
            this.db.SaveChangesAsync();
        }
    }
}
