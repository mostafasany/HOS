using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Api.Common.Domain;

namespace Nop.Plugin.Api.Common.DataMappings
{
    public class WebHooksMap : NopEntityTypeConfiguration<WebHooks>
    {
        public override void Configure(EntityTypeBuilder<WebHooks> builder)
        {
            builder.ToTable("WebHooks", "WebHooks");
            builder.HasKey(wh => new {wh.User, wh.Id});

            builder.Property(wh => wh.ProtectedData).IsRequired();
            builder.Property(wh => wh.RowVer).IsRowVersion();
        }
    }
}