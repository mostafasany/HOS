using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Content.Modules.HomeCarousel.Dto
{
    public class CarouselRootObject : ISerializableObject
    {
        public CarouselRootObject()
        {
            Carousel = new List<CarouselDto>();
        }

        [JsonProperty("carousel")] public IList<CarouselDto> Carousel { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "Carousel";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(CarouselDto);
        }
    }
}