namespace MoneySaver.Data.Models
{
    public class ProductShoplist
    {
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int ShoplistId { get; set; }

        public virtual Shoplist Shoplist { get; set; }
    }
}