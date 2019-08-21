using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
}