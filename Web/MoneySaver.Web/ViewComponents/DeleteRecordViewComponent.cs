namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class DeleteRecordViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string recordId)
        {
            DeleteRecordComponentViewModel viewModel = new DeleteRecordComponentViewModel();
            viewModel.Id = recordId;
            return this.View(viewModel);
        }
    }
}
