using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Plugin.MenuTree.Data
{
    public class MenuTreeObjectContext : DbContext , IDbContext
    {
        public MenuTreeObjectContext(DbContextOptions<MenuTreeObjectContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MenuTreeMap());
            modelBuilder.ApplyConfiguration(new MenuTreeItemMap());
            base.OnModelCreating(modelBuilder);

        }

        public new virtual DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public virtual string GenerateCreateScript()
        {
            return Database.GenerateCreateScript();
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql, params object[] parameters) where TQuery : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public virtual int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            using (var transaction = Database.BeginTransaction())
            {
                var result = Database.ExecuteSqlCommand(sql, parameters);
                transaction.Commit();

                return result;
            }
        }

        public virtual void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }
        public void Install()
        {
            this.ExecuteSqlScript(GenerateCreateScript());
        }

        public void Uninstall()
        {
            this.DropPluginTable("Menu.TreeItems");
            this.DropPluginTable("Menu.Tree");        
        }
    }
}
