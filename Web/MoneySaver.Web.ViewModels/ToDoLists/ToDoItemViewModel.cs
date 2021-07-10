namespace MoneySaver.Web.ViewModels.ToDoLists
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.ToDoLists.Enums;

    public class ToDoItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public StatusType Status { get; set; }
    }
}
