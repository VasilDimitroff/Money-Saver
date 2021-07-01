namespace MoneySaver.Web.ViewModels.ToDoLists
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

        public IEnumerable<string> ListItems { get; set; }
    }
}
