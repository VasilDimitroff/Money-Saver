namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;
    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;

    public interface ICategoriesService
    {
        public Task<string> AddAsync(string categoryName, int walletId, string badgeColor);

        public Task<string> RemoveAsync(int oldCategoryId, int newCategoryId);

        public Task<string> EditAsync(int categoryId, string categoryName, int walletId, string badgeColor);

        public Task<AllRecordsInCategoryDto> GetRecordsByCategoryAsync(int categoryId);

        public Task<EditCategoryDto> GetCategoryInfoForEditAsync(int categoryId);

        public Task<DeleteCategoryDto> GetCategoryInfoForDeleteAsync(int categoryId, int walletId);

        public Task<IEnumerable<WalletNameAndIdDto>> GetAllWalletsWithNameAndIdAsync(string userId);

        public Task<bool> IsUserOwnCategoryAsync(string userId, int categoryId);
    }
}
