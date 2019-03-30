using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Product.Modules.SpecificationAttributes.Model
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<SpecifcationAttributesCountParametersModel>))]
    public class SpecifcationAttributesCountParametersModel
    {
    }
}