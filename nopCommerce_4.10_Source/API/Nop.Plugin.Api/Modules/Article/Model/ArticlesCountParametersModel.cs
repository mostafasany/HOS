using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Modules.Articles.Model
{
    [ModelBinder(typeof(ParametersModelBinder<ArticlesCountParametersModel>))]
    public class ArticlesCountParametersModel : BaseArticlesParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}