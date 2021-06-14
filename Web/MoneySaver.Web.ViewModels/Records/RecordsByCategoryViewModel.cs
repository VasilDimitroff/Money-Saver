namespace MoneySaver.Web.ViewModels.Records
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class RecordsByCategoryViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public RecordTypeInputModel Type { get; set; }

        public string CreatedOn { get; set; }
    }
}
