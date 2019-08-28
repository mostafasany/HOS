using FluentValidation;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.MenuTree.Models;
using Nop.Plugin.MenuTree.Domain;

namespace Nop.Web.Areas.Admin.Validators
{
    public partial class MenuTreeValidator : BaseNopValidator<MenuTreeModel>
    {
        public MenuTreeValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is Required");
        }
    }
}