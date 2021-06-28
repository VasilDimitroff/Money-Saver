namespace MoneySaver.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Record : BaseModel<string>
    {
        [Required(ErrorMessage = "Description cannot be empty")]
        [MinLength(1)]
        [MaxLength(250)]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Category Id is required")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [Required(ErrorMessage = "Record Type cannot be empty")]
        public RecordType Type { get; set; }
    }
}
