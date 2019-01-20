using System.Collections.Generic;
using Nop.Plugin.Api.Modules.Pictures.Dto;

namespace Nop.Plugin.Api.Common.Attributes
{
    public class ImageCollectionValidationAttribute : BaseValidationAttribute
    {
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public override Dictionary<string, string> GetErrors() => _errors;

        public override void Validate(object instance)
        {
            // Images are not required so they could be null
            // and there is nothing to validate in this case
            if (instance == null)
                return;

            var imagesCollection = instance as ICollection<ImageMappingDto>;

            foreach (ImageMappingDto image in imagesCollection)
            {
                var imageValidationAttribute = new ImageValidationAttribute();

                imageValidationAttribute.Validate(image);

                Dictionary<string, string> errorsForImage = imageValidationAttribute.GetErrors();

                if (errorsForImage.Count > 0)
                {
                    _errors = errorsForImage;
                    break;
                }
            }
        }
    }
}