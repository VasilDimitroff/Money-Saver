namespace MoneySaver.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;

    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext dbContext;

        public CategoriesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string categoryName)
        {
            Category category = new Category
            {
                Name = categoryName,
            };

            await this.dbContext.Categories.AddAsync(category);
            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.SuccessfullyAddedCategory, category.Name);
        }

        public async Task<CategoryInfoDto> GetCategoryAsync(int categoryId)
        {
            CategoryInfoDto category = await this.dbContext.Categories
                .Select(categ => new CategoryInfoDto
                {
                    Name = categ.Name,
                    Id = categ.Id,
                })
                 .FirstOrDefaultAsync(x => x.Id == categoryId);

            return category;
        }

        public async Task<string> RemoveAsync(int categoryId)
        {
            Category category = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
            this.dbContext.Categories.Remove(category);

            return string.Format(GlobalConstants.SuccessfullyRemovedCategory, category.Name);
        }
    }
}
