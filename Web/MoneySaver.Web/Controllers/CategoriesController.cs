namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Common;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;

    [Authorize]
    public class CategoriesController : Controller
    {
        private const int ItemsPerPage = 12;

        private readonly ICategoriesService categoriesService;
        private readonly IWalletsService walletsService;
        private readonly UserManager<ApplicationUser> userManager;

        public CategoriesController(ICategoriesService categoriesService, IWalletsService walletsService, UserManager<ApplicationUser> userManager)
        {
            this.categoriesService = categoriesService;
            this.walletsService = walletsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Add(int id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
                {
                    throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
                }

                var model = new AddCategoryInputModel();

                var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync(user.Id);

                model.WalletId = id;
                model.WalletName = await this.walletsService.GetWalletNameAsync(id);
                model.Wallets = wallets.Select(w => new AddCategoryWalletsListViewModel
                {
                    WalletId = w.WalletId,
                    WalletName = w.WalletName,
                })
                    .ToList();

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryInputModel input)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    input.WalletName = await this.walletsService.GetWalletNameAsync(input.WalletId);
                    return this.View(input);
                }

                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                await this.categoriesService.AddAsync(input.Name, input.WalletId, input.BadgeColor);

                return this.Redirect($"/Wallets/Categories/{input.WalletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Edit(int id, int walletId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                var categoryInfo = await this.categoriesService.GetCategoryInfoForEditAsync(id);
                var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync(user.Id);
                var walletName = await this.walletsService.GetWalletNameAsync(walletId);

                var model = new EditCategoryInputModel();
                model.CategoryId = id;
                model.CategoryName = categoryInfo.CategoryName;
                model.BadgeColor = Enum.Parse<BadgeColor>(categoryInfo.BadgeColor);
                model.WalletId = walletId;
                model.WalletName = walletName;
                model.Wallets = wallets.Select(w => new EditCategoryWalletsListViewModel
                {
                    WalletId = w.WalletId,
                    WalletName = w.WalletName,
                })
                    .ToList();

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryInputModel input)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    var categoryInfo = await this.categoriesService.GetCategoryInfoForEditAsync(input.CategoryId);
                    var walletName = await this.walletsService.GetWalletNameAsync(input.WalletId);

                    input.CategoryName = categoryInfo.CategoryName;
                    input.BadgeColor = Enum.Parse<BadgeColor>(categoryInfo.BadgeColor);
                    input.WalletName = walletName;

                    return this.View(input);
                }

                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.CategoryId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                await this.categoriesService.EditAsync(input.CategoryId, input.CategoryName, input.BadgeColor.ToString());

                return this.Redirect($"/Wallets/Categories/{input.WalletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Delete(int id, int walletId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, walletId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                var model = new DeleteCategoryInputModel();

                var modelInfo = await this.categoriesService.GetCategoryInfoForDeleteAsync(id, walletId);

                model.WalletId = modelInfo.WalletId;
                model.WalletName = modelInfo.WalletName;
                model.OldCategoryId = id;
                model.OldCategoryName = modelInfo.OldCategoryName;
                model.OldCategoryBadgeColor = Enum.Parse<BadgeColor>(modelInfo.OldCategoryBadgeColor.ToString());
                model.Categories = modelInfo.Categories
                    .Select(c => new DeleteCategoryNameAndIdViewModel
                    {
                        BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor.ToString()),
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                    })
                    .ToList();

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteCategoryInputModel input)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!this.ModelState.IsValid)
                {
                    if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
                    {
                        return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                    }

                    if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.OldCategoryId))
                    {
                        return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                    }

                    var modelInfo = await this.categoriesService.GetCategoryInfoForDeleteAsync(input.OldCategoryId, input.WalletId);

                    input.WalletId = modelInfo.WalletId;
                    input.WalletName = modelInfo.WalletName;
                    input.OldCategoryName = modelInfo.OldCategoryName;
                    input.OldCategoryBadgeColor = Enum.Parse<BadgeColor>(modelInfo.OldCategoryBadgeColor.ToString());
                    input.Categories = modelInfo.Categories
                        .Select(c => new DeleteCategoryNameAndIdViewModel
                        {
                            BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor.ToString()),
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                        })
                        .ToList();

                    return this.View(input);
                }

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.OldCategoryId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.NewCategoryId) && input.NewCategoryId != -1)
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                await this.categoriesService.RemoveAsync(input.OldCategoryId, input.NewCategoryId);
                return this.Redirect($"/Wallets/Categories/{input.WalletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Details(int id, int page = 1)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
                {
                    throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
                }

                var category = await this.categoriesService.GetRecordsByCategoryAsync(id, page, ItemsPerPage);
                CategoryRecordsViewModel model = new CategoryRecordsViewModel()
                {
                    Records = category.Records
                    .Select(r => new RecordsByCategoryViewModel
                    {
                        Amount = r.Amount,
                        CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                        Description = r.Description,
                        Id = r.Id,
                        Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                    })
                    .ToList(),
                    Category = category.Category,
                    CategoryId = id,
                    Currency = category.Currency,
                    WalletId = category.WalletId,
                    WalletName = category.WalletName,
                    BadgeColor = Enum.Parse<BadgeColor>(category.BadgeColor.ToString()),
                };

                model.ItemsPerPage = ItemsPerPage;
                model.PageNumber = page;
                model.RecordsCount = this.categoriesService.GetRecordsCount(id);

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Search(int categoryId, string searchTerm, int page = 1)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, categoryId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                if (searchTerm == null)
                {
                    searchTerm = string.Empty;
                }

                var category = await this.categoriesService.GetRecordsByKeywordAsync(searchTerm, categoryId, page, ItemsPerPage);
                CategoryRecordsViewModel model = new CategoryRecordsViewModel()
                {
                    Records = category.Records
                    .Select(r => new RecordsByCategoryViewModel
                    {
                        Amount = r.Amount,
                        CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                        Description = r.Description,
                        Id = r.Id,
                        Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                    })
                    .ToList(),
                    Category = category.Category,
                    CategoryId = categoryId,
                    Currency = category.Currency,
                    WalletId = category.WalletId,
                    WalletName = category.WalletName,
                    BadgeColor = Enum.Parse<BadgeColor>(category.BadgeColor.ToString()),
                    SearchTerm = searchTerm,
                };

                model.ItemsPerPage = ItemsPerPage;
                model.PageNumber = page;
                model.RecordsCount = this.categoriesService.GetSearchRecordsCount(searchTerm, categoryId);

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> DateSorted(DateTime startDate, DateTime endDate, int categoryId, int page = 1)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, categoryId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditCategory}");
                }

                var category = await this.categoriesService.GetRecordsByDateRangeAsync(startDate, endDate, categoryId, page, ItemsPerPage);
                CategoryRecordsViewModel model = new CategoryRecordsViewModel()
                {
                    Records = category.Records
                    .Select(r => new RecordsByCategoryViewModel
                    {
                        Amount = r.Amount,
                        CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                        Description = r.Description,
                        Id = r.Id,
                        Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                    })
                    .ToList(),
                    Category = category.Category,
                    CategoryId = categoryId,
                    Currency = category.Currency,
                    WalletId = category.WalletId,
                    WalletName = category.WalletName,
                    BadgeColor = Enum.Parse<BadgeColor>(category.BadgeColor.ToString()),
                };

                var startDate12AM = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                var endDate12PM = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

                model.StartDate = startDate12AM;
                model.EndDate = endDate12PM;

                model.ItemsPerPage = ItemsPerPage;
                model.PageNumber = page;
                model.RecordsCount = this.categoriesService.GetDateSortedRecordsCount(startDate, endDate, categoryId);

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }
    }
}
