using System.ComponentModel.DataAnnotations;

namespace MoneySaver.Web.Models.Records
{
    public class RecordInputModel
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public string Type { get; set; }

    }
}
