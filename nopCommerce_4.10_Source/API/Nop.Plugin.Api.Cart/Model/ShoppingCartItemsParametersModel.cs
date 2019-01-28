using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Cart.Model
{
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsParametersModel>))]
    public class ShoppingCartItemsParametersModel : BaseShoppingCartItemsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}