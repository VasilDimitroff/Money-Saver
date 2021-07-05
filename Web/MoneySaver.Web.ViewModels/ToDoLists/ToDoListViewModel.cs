namespace MoneySaver.Web.ViewModels.ToDoLists
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MoneySaver.Web.ViewModels.ToDoLists.Enums;

    public class ToDoListViewModel : AddToDoListInputModel
    {
        public ToDoListViewModel()
        {
            this.Items = new HashSet<ToDoItemViewModel>();
        }

        [Required]
        public string Id { get; set; }

        [Required]
        public StatusType Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ItemsToShow { get; set; }

        //When viewing, addding or edit via edit view list
        public IEnumerable<ToDoItemViewModel> Items { get; set; }
    }
}
