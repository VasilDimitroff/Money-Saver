namespace MoneySaver.Data.Models
{
    using MoneySaver.Data.Common.Models;

    public class ToDoItem : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public int ToDoListId { get; set; }

        public virtual ToDoList ToDoList { get; set; }
    }
}
