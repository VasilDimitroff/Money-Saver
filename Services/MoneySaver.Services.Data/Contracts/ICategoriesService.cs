namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;
    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;

    public interface ICategoriesService
    {
        public Task<string> AddAsync(string categoryName, int walletId);

        public Task<string> RemoveAsync(int categoryId);

        public Task<AllRecordsInCategoryDto> GetRecordsByCategoryAsync(int categoryId);

    }
}
