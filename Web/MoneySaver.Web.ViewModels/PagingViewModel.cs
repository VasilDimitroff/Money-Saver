namespace MoneySaver.Web.ViewModels
{
    using System;

    public class PagingViewModel
    {
        public int PageNumber { get; set; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public int PreviousPageNumber => this.PageNumber - 1;

        public bool HasNextPage => this.PageNumber < this.PagesCount;

        public int NextPageNumber => this.PageNumber + 1;

        public int PagesCount => (int)Math.Ceiling((double)this.RecordsCount / this.ItemsPerPage);

        public int RecordsCount { get; set; }

        public int ItemsPerPage { get; set; }

        public int WalletId { get; set; }
    }
}
