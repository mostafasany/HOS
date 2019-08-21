using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Articles;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Article.Translator
{
    public class ArticleTranslator : IArticleTranslator
    {
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;

        public ArticleTranslator(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor,
            IPictureService pictureService)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }

        public ArticlesDto ToDto(FNS_Article article)
        {
            var picture = _pictureService.GetPictureById(article.PictureId);
            var imageDto = PrepareImageDto(picture);

            var articleDto = new ArticlesDto
            {
                Id = article.Id,
                AllowComments = article.AllowComments,
                CommentCount = article.CommentCount,
                CreatedOnUtc = article.CreatedOnUtc,
                UpdatedOnUtc = article.UpdatedOnUtc,
                SeName = _urlRecordService.GetSeName(article),
                Title = _localizationService.GetLocalized(article, x => x.Title, _currentLanguageId),
                Tags = _localizationService.GetLocalized(article, x => x.Tags, _currentLanguageId),
                Body = _localizationService.GetLocalized(article, x => x.Body, _currentLanguageId),
                MetaDescription =
                    _localizationService.GetLocalized(article, x => x.MetaDescription, _currentLanguageId),
                MetaTitle = _localizationService.GetLocalized(article, x => x.MetaTitle, _currentLanguageId)
            };
            if (imageDto != null) articleDto.Image = imageDto;

            return articleDto;
        }

        public ArticleGroupDto ToGroupDto(FNS_ArticleGroup articleGroup)
        {
            return new ArticleGroupDto
            {
                Id = articleGroup.Id, Name = articleGroup.Name, ParentGroupId = articleGroup.ParentGroupId
            };
        }

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