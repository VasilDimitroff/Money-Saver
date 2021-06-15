namespace MoneySaver.Data.Models
{
    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Record : BaseModel<int>
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public RecordType Type { get; set; }
    }
}
