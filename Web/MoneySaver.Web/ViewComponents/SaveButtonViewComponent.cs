namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class SaveButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return this.View();
        }
    }
}
