namespace MoneySaver.Data.Models
{
    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Record : BaseDeletableModel<string>
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public RecordType Type { get; set; }
    }
}
