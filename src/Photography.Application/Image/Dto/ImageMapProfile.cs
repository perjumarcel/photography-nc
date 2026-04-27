using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Photography.Image.Dto
{
    public class ImageMapProfile : Profile
    {
        public ImageMapProfile()
        {
            CreateMap<ImageDto, Entities.Image>();
            CreateMap<CreateImageDto, Entities.Image>();
        }
    }
}
