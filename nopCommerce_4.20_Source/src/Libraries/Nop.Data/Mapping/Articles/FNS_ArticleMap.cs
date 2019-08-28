using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Articles;

namespace Nop.Data.Mapping.Articles
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class FNS_ArticleMap : NopEntityTypeConfiguration<Core.Domain.Articles.FNS_Article>
    {
        public static string ArticlesTable => "FNS_Article";

        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Core.Domain.Articles.FNS_Article> builder)
        {
            builder.ToTable(ArticlesTable);
            builder.HasKey(article => article.Id);

            base.Configure(builder);
        }

        #endregion
    }

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

            builder.HasOne(productCategory => productCategory.FnsArticle)
                .WithMany(category => category.ArticleCategories)
                .HasForeignKey(productCategory => productCategory.ArticleId)
                .IsRequired();


            base.Configure(builder);
        }

        #endregion
    }

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

            builder.HasOne(productCategory => productCategory.FnsArticle)
                .WithMany(groups => groups.ArticleGroups)
                .HasForeignKey(productCategory => productCategory.ArticleId)
                .IsRequired();

            builder.HasOne(productCategory => productCategory.Group)
                .WithMany()
                .HasForeignKey(productCategory => productCategory.GroupId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }

}