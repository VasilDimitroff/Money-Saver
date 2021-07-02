namespace MoneySaver.Services.Data.Models.ToDoLists
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Data.Models.Enums;

    public class ToDoListDto
    {
        public ToDoListDto()
        {
            this.ListItems = new HashSet<ToDoItemDto>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public StatusType Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<ToDoItemDto> ListItems { get; set; }
    }
}
