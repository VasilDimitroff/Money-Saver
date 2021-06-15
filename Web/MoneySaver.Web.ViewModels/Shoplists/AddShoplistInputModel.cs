namespace MoneySaver.Web.ViewModels.Shoplists
{
    using System.Collections.Generic;

    public class AddShoplistInputModel
    {
        public int ShoplistId { get; set; }

        public IEnumerable<string> Products { get; set; }
    }
}
