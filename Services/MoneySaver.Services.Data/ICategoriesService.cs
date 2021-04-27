namespace MoneySaver.Services.Data
{
    public interface ICategoriesService
    {
        void Add(string name);

        void Remove(string name);
    }
}
