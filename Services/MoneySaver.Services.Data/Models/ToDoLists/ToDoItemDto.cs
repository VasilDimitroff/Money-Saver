namespace MoneySaver.Services.Data.Models.ToDoLists
{
    using MoneySaver.Data.Models.Enums;

    public class ToDoItemDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public StatusType Status { get; set; }
    }
}