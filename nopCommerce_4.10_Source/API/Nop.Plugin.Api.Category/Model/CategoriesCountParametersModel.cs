using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Category.Model
{
    [ModelBinder(typeof(ParametersModelBinder<CategoriesCountParametersModel>))]
    public class CategoriesCountParametersModel : BaseCategoriesParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}