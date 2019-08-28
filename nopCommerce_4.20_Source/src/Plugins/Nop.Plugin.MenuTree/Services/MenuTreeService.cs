using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.MenuTree.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Articles;

namespace Nop.Plugin.MenuTree.Services
{
    public class MenuTreeService : IMenuTreeService
    {
        private readonly IRepository<Domain.MenuTree> _menutreeRepository;
        private readonly IRepository<Domain.MenuTreeItem> _menutreeitemRepository;
        private readonly IRepository<FNS_Article> _fnsarticleRepository;

        public MenuTreeService(IRepository<Domain.MenuTree> menutreeRepository
            , IRepository<Domain.MenuTreeItem> menutreeitemRepository
            , IRepository<FNS_Article> fnsarticleRepository
            )
        {
            _menutreeRepository = menutreeRepository;
            _menutreeitemRepository = menutreeitemRepository;
            _fnsarticleRepository = fnsarticleRepository;
        }

        public void DeleteMenuTree(Domain.MenuTree menuTree)
        {
            if (menuTree == null)
                throw new ArgumentNullException(nameof(menuTree));

            _menutreeRepository.Update(menuTree);
        }

        public MenuTreeItem FindMenuTreeItem(IList<MenuTreeItem> source, int itemId, int menutreeId)
        {
            foreach (var menutreeitem in source)
                if (menutreeitem.ItemId == itemId && menutreeitem.MenuTreeId == menutreeId)
                    return menutreeitem;

            return null;
        }

        public IPagedList<FNS_Article> GetAllArticles(string articlename, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from art in _fnsarticleRepository.Table
                        select art;
            if (articlename != null)
            {
                query = query.Where(art => art.Title.Contains(articlename));
            }
            query = query.Where(mt => mt.Deleted == false);
            var records = new PagedList<FNS_Article>(query, pageIndex, pageSize);
            return records;
        }

        public IPagedList<Domain.MenuTree> GetAllMenutree(string menutreename, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from mt in _menutreeRepository.Table
                        select mt;
            if(menutreename!=null)
            {
                query = query.Where(mt => mt.Name.Contains(menutreename));
            }
            query = query.Where(mt => mt.Deleted==false);
            var records = new PagedList<Domain.MenuTree>(query, pageIndex, pageSize);
            return records;
        }

        public FNS_Article GetArticleById(int id)
        {
            var query = from art in _fnsarticleRepository.Table
                        select art;
            query = query.Where(art => art.Deleted == false && art.Id == id);

            return query.FirstOrDefault();
        }

        public List<FNS_Article> GetArticlesByIds(int[] articlesIds)
        {
            if (articlesIds == null || articlesIds.Length == 0)
                return new List<FNS_Article>();

            var query = from art in _fnsarticleRepository.Table
                        where articlesIds.Contains(art.Id) && !art.Deleted
                        select art;

            return query.ToList();
        }

        public IPagedList<MenuTreeItem> GetItemMenutreesByMenutreeId(int menuTreeId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (menuTreeId == 0)
                return new PagedList<MenuTreeItem>(new List<MenuTreeItem>(), pageIndex, pageSize);
            var menutree = GetMenutreeById(menuTreeId);

             if (menutree != null && menutree.EntityName == "Brand")
            {
                var query = from m in _menutreeitemRepository.Table
                            where !m.Deleted  && m.MenuTreeId == menuTreeId
                            orderby m.DisplayOrder, m.Id
                            select m;
                var menutreeitems = new PagedList<MenuTreeItem>(query, pageIndex, pageSize);
                return menutreeitems;
            }
            else if (menutree != null && menutree.EntityName == "Category")
            {
                var query = from m in _menutreeitemRepository.Table
                            where !m.Deleted && m.MenuTreeId == menuTreeId
                            orderby m.DisplayOrder, m.Id
                            select m;
                var menutreeitems = new PagedList<MenuTreeItem>(query, pageIndex, pageSize);
                return menutreeitems;
            }
            else if (menutree != null && menutree.EntityName == "Article")
            {
                var query = from m in _menutreeitemRepository.Table
                            where !m.Deleted && m.MenuTreeId == menuTreeId
                            orderby m.DisplayOrder, m.Id
                            select m;
                var menutreeitems = new PagedList<MenuTreeItem>(query, pageIndex, pageSize);
                return menutreeitems;
            }
            else
            {
                var query = from m in _menutreeitemRepository.Table
                            where !m.Deleted && m.MenuTreeId == menuTreeId
                            orderby m.DisplayOrder, m.Id
                            select m;
                var menutreeitems = new PagedList<MenuTreeItem>(query, pageIndex, pageSize);
                return menutreeitems;
            }
        }

        public Domain.MenuTree GetMenutreeById(int id)
        {
            var query = from mt in _menutreeRepository.Table
                        select mt;
            query = query.Where(mt => mt.Deleted == false && mt.Id == id);

            return query.FirstOrDefault();
        }

        public MenuTreeItem GetMenuTreeItemById(int menuTreeItemId)
        {
            if (menuTreeItemId == 0)
                return null;

            return _menutreeitemRepository.GetById(menuTreeItemId);
        }

        public void InsertMenuTree(Domain.MenuTree menuTree)
        {
            if (menuTree == null)
                throw new ArgumentNullException(nameof(menuTree));

            _menutreeRepository.Insert(menuTree);
        }

        public void InsertMenuTreeItem(MenuTreeItem menuTreeItem)
        {
            if (menuTreeItem == null)
                throw new ArgumentNullException(nameof(menuTreeItem));

            _menutreeitemRepository.Insert(menuTreeItem);
        }

        public List<Domain.MenuTree> Menutree()
        {
            var query = from mt in _menutreeRepository.Table
                        select mt;
            query = query.Where(mt => mt.Deleted == false);
           
            return query.ToList();
        }

        public void UpdateMenuTree(Domain.MenuTree menuTree)
        {
            if (menuTree == null)
                throw new ArgumentNullException(nameof(menuTree));

            _menutreeRepository.Update(menuTree);
        }

        public void UpdateMenuTreeItem(MenuTreeItem menuTreeItem)
        {
            if (menuTreeItem == null)
                throw new ArgumentNullException(nameof(menuTreeItem));

            _menutreeitemRepository.Update(menuTreeItem);

        }


    }
}
