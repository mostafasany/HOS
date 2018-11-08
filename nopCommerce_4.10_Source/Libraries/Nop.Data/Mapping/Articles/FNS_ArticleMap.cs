using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Articles;

namespace Nop.Data.Mapping.Articles
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class FNS_ArticleMap : NopEntityTypeConfiguration<FNS_Article>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FNS_Article> builder)
        {
            builder.ToTable(NopMappingDefaults.ArticlesTable);
            builder.HasKey(article => article.Id);

            base.Configure(builder);
        }

        #endregion
    }
}