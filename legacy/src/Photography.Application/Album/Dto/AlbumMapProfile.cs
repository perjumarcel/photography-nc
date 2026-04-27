using System.Linq;
using Abp.AutoMapper;
using AutoMapper;
using Photography.Entities;

namespace Photography.Album.Dto
{
    public class AlbumMapProfile : Profile
    {
        public AlbumMapProfile()
        {
            CreateMap<AlbumDto, Entities.Album>()
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ForMember(x => x.Category, opt => opt.Ignore())
                .ForMember(x => x.CreatorUserId, opt => opt.Ignore())
                .ForMember(x => x.CreationTime, opt => opt.Ignore());

            CreateMap<Entities.Album, AlbumDto>()
                .ForMember(x => x.Category, opt => opt.MapFrom(album => album.Category.Title));

            CreateMap<Entities.Album, AlbumDdDto>()
                .ForMember(x => x.Category, opt => opt.MapFrom(album => album.Category.Title));

            CreateMap<Entities.Album, AlbumListItemDto>()
                .ForMember(x => x.Category, opt => opt.MapFrom(album => album.Category.Title))
                .ForMember(x => x.Cover, opt => opt.MapFrom(album => album.Images.FirstOrDefault(i => i.ImageType == ImageType.Cover) ?? album.Images.FirstOrDefault(i => i.ImageOrientation == ImageOrientation.Horizontal)));

            CreateMap<CreateAlbumDto, Entities.Album>()
                .ForMember(x => x.Images, opt => opt.Ignore());
        }
    }
}
