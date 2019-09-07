using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Custom;


namespace Nop.Data.Mapping.Custom
{
    public class MenuTreeMap : NopEntityTypeConfiguration<MenuTree>
    {

        public override void Configure(EntityTypeBuilder<MenuTree> builder)
        {
            builder.ToTable("Menu.Tree");
            builder.HasKey(record => record.Id);
        }
    }

    public class MenuTreeItemMap : NopEntityTypeConfiguration<MenuTreeItem>
    {
        public override void Configure(EntityTypeBuilder<MenuTreeItem> builder)
        {
            builder.ToTable("Menu.TreeItems");
            builder.HasKey(MenuTreeItem => MenuTreeItem.Id);

            builder.HasOne(MenuTreeItem => MenuTreeItem.menuTree)
                .WithMany()
                .HasForeignKey(MenuTreeItem => MenuTreeItem.MenuTreeId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
