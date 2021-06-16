namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;

    public class BackButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return this.View();
        }
    }
}
