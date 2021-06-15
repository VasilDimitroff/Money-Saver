namespace MoneySaver.Web.ViewModels.Categories
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoryNameIdViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string WalletName { get; set; }

        public BadgeColor BadgeColor { get; set; }
    }
}
