using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Articles;

namespace Nop.Data.Mapping.Articles
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class FNS_ArticleCategoryMap : NopEntityTypeConfiguration<FNS_ArticleCategory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FNS_ArticleCategory> builder)
        {
            builder.ToTable(nameof(FNS_ArticleCategory));
            builder.HasKey(articleGroupMap => articleGroupMap.Id);

        
            builder.HasOne(productCategory => productCategory.Category)
                .WithMany()
                .HasForeignKey(productCategory => productCategory.CategoryId)
                .IsRequired();

            builder.HasOne(productCategory => productCategory.Article)
                .WithMany(category => category.ArticleCategories)
                .HasForeignKey(productCategory => productCategory.ArticleId)
                .IsRequired();


            base.Configure(builder);
        }

        #endregion
    }
}