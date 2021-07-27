namespace MoneySaver.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MoneySaver.Data.Models;

    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> company)
        {
            company
                .HasKey(c => new { c.Id });

            company
                .HasIndex(c => c.Ticker).IsUnique(true);
        }
    }
}
