namespace MoneySaver.Web.ViewModels.Categories
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoryStatisticsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TotalRecordsCount { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalExpensesAmount { get; set; }

        public decimal TotalIncomesAmount { get; set; }

        public string ModifiedOn { get; set; }
    }
}
