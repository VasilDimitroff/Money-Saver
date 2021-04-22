namespace MoneySaver.Data.Configurations
{
    using MoneySaver.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProductShoplistConfiguration : IEntityTypeConfiguration<ProductShoplist>
    {
        public void Configure(EntityTypeBuilder<ProductShoplist> productShoplist)
        {
            productShoplist.HasKey(ps => new { ps.ShoplistId, ps.ProductId });

            productShoplist.HasOne(ps => ps.Product)
                .WithMany(product => product.ProductShoplists)
                .HasForeignKey(ps => ps.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            productShoplist.HasOne(ps => ps.Shoplist)
               .WithMany(shoplist => shoplist.ProductsShoplist)
               .HasForeignKey(ps => ps.ShoplistId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
