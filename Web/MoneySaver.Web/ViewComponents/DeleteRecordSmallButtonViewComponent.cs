namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class DeleteRecordSmallButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string recordId)
        {
            DeleteRecordSmallButtonComponentViewModel viewModel = new DeleteRecordSmallButtonComponentViewModel();
            viewModel.RecordId = recordId;
            return this.View(viewModel);
        }
    }
}
