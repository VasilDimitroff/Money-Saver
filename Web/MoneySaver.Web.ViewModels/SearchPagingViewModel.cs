namespace MoneySaver.Web.ViewModels
{
    using System;

    public class SearchPagingViewModel : PagingViewModel
    {
        public string SearchTerm { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
