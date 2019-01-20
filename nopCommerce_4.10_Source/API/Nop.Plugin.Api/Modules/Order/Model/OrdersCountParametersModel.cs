using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Modules.Orders.Model
{
    [ModelBinder(typeof(ParametersModelBinder<OrdersCountParametersModel>))]
    public class OrdersCountParametersModel : BaseOrdersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}