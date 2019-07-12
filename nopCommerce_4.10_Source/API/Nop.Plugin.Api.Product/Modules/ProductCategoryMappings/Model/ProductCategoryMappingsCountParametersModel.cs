using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Product.Modules.ProductCategoryMappings.Model
{
    [ModelBinder(typeof(ParametersModelBinder<ProductCategoryMappingsCountParametersModel>))]
    public class ProductCategoryMappingsCountParametersModel : BaseCategoryMappingsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}