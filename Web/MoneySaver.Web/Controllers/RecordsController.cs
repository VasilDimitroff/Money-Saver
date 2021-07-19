namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Common;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;
    using MoneySaver.Web.ViewModels.Wallets;

    [Authorize]
    public class RecordsController : Controller
    {
        private readonly IRecordsService recordsService;
        private readonly IWalletsService walletsService;
        private readonly UserManager<ApplicationUser> userManager;

        public RecordsController(
            IRecordsService recordsService,
            IWalletsService walletsService,
            UserManager<ApplicationUser> userManager)
        {
            this.recordsService = recordsService;
            this.walletsService = walletsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Add(int walletId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, walletId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                AddRecordViewModel model = new AddRecordViewModel();

                var categories = await this.walletsService.GetWalletCategoriesAsync(walletId);

                model.Categories = categories.Select(x => new CategoryNameIdViewModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    WalletName = x.WalletName,
                    BadgeColor = Enum.Parse<BadgeColor>(x.BadgeColor.ToString()),
                });

                model.WalletId = walletId;
                model.WalletName = await this.walletsService.GetWalletNameAsync(walletId);

                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRecordViewModel input)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            try
            {
                if (!this.ModelState.IsValid)
                {
                    if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
                    {
                        return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                    }

                    var categories = await this.walletsService.GetWalletCategoriesAsync(input.WalletId);

                    input.Categories = categories.Select(x => new CategoryNameIdViewModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        WalletName = x.WalletName,
                        BadgeColor = Enum.Parse<BadgeColor>(x.BadgeColor.ToString()),
                    });

                    input.WalletName = await this.walletsService.GetWalletNameAsync(input.WalletId);

                    return this.View(input);
                }

                if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForEditWallet}");
                }

                var enumValueAsString = input.Type.ToString();
                await this.recordsService.AddAsync(input.CategoryId, input.Description, input.Amount, enumValueAsString, input.CreatedOn);

                return this.Redirect($"/Wallets/Records/{input.WalletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        public async Task<IActionResult> Edit(string id, int walletId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.recordsService.IsUserOwnRecordAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditRecord}");
                }

                var recordDto = await this.recordsService.GetRecordWithAllCategories(id, walletId);

                EditRecordViewModel model = new EditRecordViewModel()
                {
                    Id = recordDto.Id,
                    Description = recordDto.Description,
                    ModifiedOn = recordDto.ModifiedOn,
                    CreatedOn = recordDto.CreatedOn,
                    Type = Enum.Parse<RecordTypeInputModel>(recordDto.Type.ToString()),
                    WalletId = walletId,
                    Amount = recordDto.Amount,
                    OldAmount = 0,
                    WalletName = recordDto.WalletName,
                    CategoryId = recordDto.CategoryId,
                    Categories = recordDto.Categories.Select(c => new CategoryNameIdViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        WalletName = c.WalletName,
                    })
                    .ToList(),
                };
                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditRecordInputModel input)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!this.ModelState.IsValid)
                {
                    if (!await this.recordsService.IsUserOwnRecordAsync(user.Id, input.Id))
                    {
                        return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditRecord}");
                    }

                    var recordDto = await this.recordsService.GetRecordWithAllCategories(input.Id, input.WalletId);

                    EditRecordViewModel model = new EditRecordViewModel()
                    {
                        Id = recordDto.Id,
                        Description = recordDto.Description,
                        ModifiedOn = recordDto.ModifiedOn,
                        CreatedOn = recordDto.CreatedOn,
                        Type = Enum.Parse<RecordTypeInputModel>(recordDto.Type.ToString()),
                        WalletId = input.WalletId,
                        Amount = recordDto.Amount,
                        OldAmount = 0,
                        WalletName = recordDto.WalletName,
                        CategoryId = recordDto.CategoryId,
                        Categories = recordDto.Categories.Select(c => new CategoryNameIdViewModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            WalletName = c.WalletName,
                        })
                    .ToList(),
                    };

                    return this.View(model);
                }

                if (!await this.recordsService.IsUserOwnRecordAsync(user.Id, input.Id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditRecord}");
                }

                await this.recordsService
                    .UpdateRecord(
                    input.Id,
                    input.CategoryId,
                    input.WalletId,
                    input.Description,
                    input.OldAmount,
                    input.Amount,
                    input.Type.ToString(),
                    input.CreatedOn.HasValue ? input.CreatedOn.Value : DateTime.UtcNow);

                return this.Redirect($"/Wallets/Records/{input.WalletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                if (!await this.recordsService.IsUserOwnRecordAsync(user.Id, id))
                {
                    return this.Redirect($"/Home/Error?message={GlobalConstants.NoPermissionForViewOrEditRecord}");
                }

                int walletId = await this.walletsService.GetWalletIdByRecordIdAsync(id);
                await this.recordsService.RemoveAsync(id);

                // return this.RedirectToAction("All", "Records", new { walletId, action = "Submit", submitAll = false });
                return this.Redirect($"/Wallets/Records/{walletId}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }
    }
}
