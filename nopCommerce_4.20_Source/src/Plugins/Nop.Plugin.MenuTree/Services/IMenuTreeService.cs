using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.MenuTree.Domain;
using Nop.Core.Domain.Articles;

namespace Nop.Plugin.MenuTree.Services
{
   public partial interface IMenuTreeService
    {
        IPagedList<Domain.MenuTree> GetAllMenutree(string menutreename,
           int pageIndex = 0, int pageSize = int.MaxValue);

        List<Domain.MenuTree> Menutree();

        Domain.MenuTree GetMenutreeById(int id);

        void InsertMenuTree(Domain.MenuTree menuTree);

        void DeleteMenuTree(Domain.MenuTree menuTree);

        void UpdateMenuTree(Domain.MenuTree menuTree);

        IPagedList<Domain.MenuTreeItem> GetItemMenutreesByMenutreeId(int menuTreeId,
           int pageIndex = 0, int pageSize = int.MaxValue);

       MenuTreeItem GetMenuTreeItemById(int menuTreeItemId);

        void UpdateMenuTreeItem(MenuTreeItem menuTreeItem);

        MenuTreeItem FindMenuTreeItem(IList<MenuTreeItem> source, int itemId, int menutreeId);

        void InsertMenuTreeItem(Domain.MenuTreeItem menuTreeItem);

        IPagedList<FNS_Article> GetAllArticles(string articlename, int pageIndex = 0, int pageSize = int.MaxValue);

        List<FNS_Article> GetArticlesByIds(int[] articlesIds);

        FNS_Article GetArticleById(int id);



    }
}
