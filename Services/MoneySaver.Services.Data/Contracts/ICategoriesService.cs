namespace MoneySaver.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface ICategoriesService
    {
        public Task<string> AddAsync(string categoryName, int walletId);

        public Task<string> RemoveAsync(int categoryId);

        public Task<CategoryInfoDto> GetCategoryAsync(int categoryId);
    }
}
