namespace MoneySaver.Data.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class ToDoList : BaseModel<string>
    {
        public ToDoList()
        {
            this.ListItems = new HashSet<ToDoItem>();
        }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ToDoItem> ListItems { get; set; }
    }
}
