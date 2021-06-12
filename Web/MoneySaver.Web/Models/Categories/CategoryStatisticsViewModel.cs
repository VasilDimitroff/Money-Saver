namespace MoneySaver.Web.Models.Categories
{
    public class CategoryStatisticsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TotalRecordsCount { get; set; }

        public decimal TotalExpensesAmount { get; set; }
        public decimal TotalIncomesAmount { get; set; }
    }
}
