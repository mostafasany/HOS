using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Articles;

namespace Nop.Data.Mapping.Articles
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class FNS_ArticleGroupMap : NopEntityTypeConfiguration<FNS_ArticleGroup_Mapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FNS_ArticleGroup_Mapping> builder)
        {
            builder.ToTable(nameof(FNS_ArticleGroup_Mapping));
            builder.HasKey(articleGroupMap => articleGroupMap.Id);

            builder.HasOne(productCategory => productCategory.Article)
                .WithMany()
                .HasForeignKey(productCategory => productCategory.ArticleId)
                .IsRequired();

            builder.HasOne(productCategory => productCategory.Group)
                //.WithMany(product => product.)
                .WithMany()
                .HasForeignKey(productCategory => productCategory.GroupId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}