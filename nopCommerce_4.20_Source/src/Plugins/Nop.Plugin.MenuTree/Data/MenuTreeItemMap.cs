using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Nop.Plugin.MenuTree.Data
{
    public class MenuTreeItemMap : NopEntityTypeConfiguration<Domain.MenuTreeItem>
    {
        public override void Configure(EntityTypeBuilder<Domain.MenuTreeItem> builder)
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
