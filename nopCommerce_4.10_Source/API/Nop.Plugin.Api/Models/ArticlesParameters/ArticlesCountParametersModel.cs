using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ArticlesParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ArticlesCountParametersModel>))]
    public class ArticlesCountParametersModel : BaseArticlesParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}