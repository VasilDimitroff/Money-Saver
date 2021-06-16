namespace MoneySaver.Web.ViewModels.Shoplists
{
    using System.Collections.Generic;

    public class AddToDoListInputModel
    {
        public string Name { get; set; }

        public IEnumerable<string> ListItems { get; set; }
    }
}
