namespace MoneySaver.Services.Data
{
    public interface ISettingsService
    {
        int GetCount();

        object GetAll<T>();
    }
}
