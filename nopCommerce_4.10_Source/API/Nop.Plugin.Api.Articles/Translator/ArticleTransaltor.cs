using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Articles;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Article.Translator
{
    public class ArticleTransaltor : IArticleTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;

        public ArticleTransaltor(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor, IPictureService pictureService)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                StringValues lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public ArticlesDto PrepateArticleDto(Core.Domain.Articles.Article article)
        {
            Picture picture = _pictureService.GetPictureById(article.PictureId);
            ImageDto imageDto = PrepareImageDto(picture);

            var articleDto = new ArticlesDto
            {
                Id = article.Id,
                AllowComments = article.AllowComments,
                CommentCount = article.CommentCount,
                CreatedOnUtc = article.CreatedOnUtc,
                UpdatedOnUtc = article.UpdatedOnUtc,
                SeName = _urlRecordService.GetSeName(article)
            };
            articleDto.Title = _localizationService.GetLocalized(article, x => x.Title, _currentLangaugeId);
            articleDto.Tags = _localizationService.GetLocalized(article, x => x.Tags, _currentLangaugeId);
            articleDto.Body = _localizationService.GetLocalized(article, x => x.Body, _currentLangaugeId);
            articleDto.MetaDescription = _localizationService.GetLocalized(article, x => x.MetaDescription, _currentLangaugeId);
            articleDto.MetaTitle = _localizationService.GetLocalized(article, x => x.MetaTitle, _currentLangaugeId);
            if (imageDto != null) articleDto.Image = imageDto;

            return articleDto;
        }

        public ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup articleGroup) => new ArticleGroupDto {Id = articleGroup.Id, Name = articleGroup.Name, ParentGroupId = articleGroup.ParentGroupId};

        protected ImageDto PrepareImageDto(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
                image = new ImageDto
                {
                    //Attachment = Convert.ToBase64String(picture.PictureBinary),
                    Src = _pictureService.GetPictureUrl(picture)
                };

            return image;
        }
    }
}