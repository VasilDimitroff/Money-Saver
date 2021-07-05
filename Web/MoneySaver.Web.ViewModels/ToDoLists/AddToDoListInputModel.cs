﻿namespace MoneySaver.Web.ViewModels.ToDoLists
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddToDoListInputModel
    {
        public AddToDoListInputModel()
        {
            this.ListItems = new HashSet<string>();
        }

        [Required]
        public string Name { get; set; }

        //When adding new list or when fill list
        public IEnumerable<string> ListItems { get; set; }
    }
}
