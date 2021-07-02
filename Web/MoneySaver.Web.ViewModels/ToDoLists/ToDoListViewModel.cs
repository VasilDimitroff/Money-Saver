namespace MoneySaver.Web.ViewModels.ToDoLists
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.ToDoLists.Enums;

    public class ToDoListViewModel
    {
        public ToDoListViewModel()
        {
            this.ListItems = new HashSet<ToDoItemViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public StatusType Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<ToDoItemViewModel> ListItems { get; set; }
    }
}
