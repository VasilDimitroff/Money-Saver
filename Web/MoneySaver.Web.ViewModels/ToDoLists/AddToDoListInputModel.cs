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

        [Required(ErrorMessage = "Please enter a list name")]
        public string Name { get; set; }

        // When adding new list or when fill list
        public ICollection<string> ListItems { get; set; }
    }
}
