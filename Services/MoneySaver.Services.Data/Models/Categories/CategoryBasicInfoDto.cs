namespace MoneySaver.Services.Data.Models.Categories
{
    using MoneySaver.Data.Models.Enums;

    public class CategoryBasicInfoDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string WalletName { get; set; }

        public BadgeColor BadgeColor { get; set; }
    }
}
