using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Product.Modules.Product.Model
{
    [ModelBinder(typeof(ParametersModelBinder<ProductsCountParametersModel>))]
    public class ProductsCountParametersModel : BaseProductsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}