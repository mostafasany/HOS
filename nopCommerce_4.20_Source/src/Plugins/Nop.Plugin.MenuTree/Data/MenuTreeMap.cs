using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Nop.Plugin.MenuTree.Data
{
    public class MenuTreeMap : NopEntityTypeConfiguration<Domain.MenuTree>
    {

        public override void Configure(EntityTypeBuilder<Domain.MenuTree> builder)
        {
            builder.ToTable("Menu.Tree");
            builder.HasKey(record => record.Id);

        }


    }
}
