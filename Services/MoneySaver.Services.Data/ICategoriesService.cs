namespace MoneySaver.Services.Data
{
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        public Task<string> AddAsync(string userId, string name);

        public Task<string> RemoveAsync(string userId, string name);
    }
}
