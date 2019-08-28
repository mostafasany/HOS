using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MenuTree.Models
{
    public class MenuTreeModel : BaseNopEntityModel , ILocalizedModel<MenuTreeLocalizedModel>
    {
        public MenuTreeModel()
        {
            AvailableEntities = new List<SelectListItem>();
            AvailableDisplayOptions = new List<SelectListItem>();
            AvailableMenuTree = new List<SelectListItem>();
            Locales = new List<MenuTreeLocalizedModel>();

            MenuTreeItemSearchModel = new MenuTreeItemSearchModel();

        }
        public int Id { get; set; }

        [NopResourceDisplayName("Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Parent")]
        public int? ParentId { get; set; }

        [NopResourceDisplayName("Entity Name")]
        public string EntityName { get; set; }

        [NopResourceDisplayName("Display In")]
        public string DisplayIn { get; set; }

        public bool Deleted { get; set; }

        [NopResourceDisplayName("Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Created On")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Updated On")]
        public DateTime UpdatedOnUtc { get; set; }

        [NopResourceDisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Show On Homepage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("SeName")]
        public string SeName { get; set; }

        [NopResourceDisplayName("Entity Name")]
        public List<SelectListItem> AvailableEntities { get; set; }

        [NopResourceDisplayName("Display In")]
        public List<SelectListItem> AvailableDisplayOptions { get; set; }

        public List<SelectListItem> AvailableMenuTree { get; set; }

        [NopResourceDisplayName("Locales")]
        public IList<MenuTreeLocalizedModel> Locales { get; set; }

        public MenuTreeItemSearchModel MenuTreeItemSearchModel { get; set; }

    }
    public partial class MenuTreeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("SeName")]
        public string SeName { get; set; }
    }
}
