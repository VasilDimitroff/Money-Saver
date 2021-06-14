namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;
    using MoneySaver.Web.ViewModels.Wallets;

    public class RecordsController : Controller
    {
        private readonly IRecordsService recordsService;
        private readonly IWalletsService walletsService;

        public RecordsController(IRecordsService recordsService, IWalletsService walletsService)
        {
            this.recordsService = recordsService;
            this.walletsService = walletsService;
        }

        public async Task<IActionResult> Add(int walletId)
        {
            AddRecordViewModel model = new AddRecordViewModel();

            var categories = await this.walletsService.GetWalletCategoriesAsync(walletId);

            model.Categories = categories.Select(x => new CategoryNameIdViewModel()
            {
                Name = x.Name,
                Id = x.Id,
                WalletName = x.WalletName,
            });

            model.WalletName = await this.walletsService.GetWalletNameAsync(walletId);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRecordViewModel input)
        {
            var enumValueAsString = input.Type.ToString();
            await this.recordsService.AddAsync(input.CategoryId, input.Description, input.Amount, enumValueAsString);
            return this.Redirect($"/Wallets/Records/{input.WalletId}");
        }

        public async Task<IActionResult> Edit(string id, int walletId)
        {
            if (id == null)
            {
                return this.Redirect($"/Wallets/Records/{walletId}");
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

        [HttpPost]
        public async Task<ActionResult> Edit(EditRecordInputModel input)
        {
            await this.recordsService.UpdateRecord(input.Id, input.CategoryId, input.WalletId, input.Description, input.OldAmount, input.Amount, input.Type, input.CreatedOn);
            return this.Redirect($"/Wallets/Records/{input.WalletId}");
        }

        public async Task<IActionResult> Delete(string recordId)
        {
           int walletId = await this.walletsService.GetWalletIdByRecordIdAsync(recordId);
           await this.recordsService.RemoveAsync(recordId);

            // return this.RedirectToAction("All", "Records", new { walletId, action = "Submit", submitAll = false });
           return this.Redirect($"/Wallets/Records/{walletId}");
        }
    }
}
