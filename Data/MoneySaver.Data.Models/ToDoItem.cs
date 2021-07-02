namespace MoneySaver.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class ToDoItem : BaseModel<string>
    {
        [Required]
        public string Name { get; set; }

        public string ToDoListId { get; set; }

        public StatusType Status { get; set; }

        public virtual ToDoList ToDoList { get; set; }
    }
}
