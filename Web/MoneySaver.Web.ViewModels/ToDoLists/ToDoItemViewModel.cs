namespace MoneySaver.Web.ViewModels.ToDoLists
{
    using MoneySaver.Web.ViewModels.ToDoLists.Enums;

    public class ToDoItemViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public StatusType Status { get; set; }
    }
}