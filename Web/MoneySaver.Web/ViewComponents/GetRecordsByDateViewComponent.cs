namespace MoneySaver.Web.ViewComponents
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.Models.ViewComponents;

    public class GetRecordsByDateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DateTime startDate, DateTime endDate, int walletId)
        {
            GetRecordsByDateComponentViewModel viewModel = new GetRecordsByDateComponentViewModel();
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.WalletId = walletId;

            return this.View(viewModel);
        }
    }
}
