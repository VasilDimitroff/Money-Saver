using MoneySaver.Data.Common.Models;
using System.Collections.Generic;

namespace MoneySaver.Data.Models
{
    public class ToDoItem : BaseDeletableModel<int>
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int ToDoListId { get; set; }

        public virtual ToDoList ToDoList { get; set; }
    }
}
