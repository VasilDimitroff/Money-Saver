namespace MoneySaver.Services.Data.Models.Categories
{
    using MoneySaver.Data.Models.Enums;

    public class DeleteCategoryNameAndIdDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }
    }
}
