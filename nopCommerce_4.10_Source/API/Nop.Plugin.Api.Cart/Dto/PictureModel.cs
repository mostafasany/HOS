namespace Nop.Plugin.Api.Modules.Cart.Dto
{
    public class PictureModel : BaseNopModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}