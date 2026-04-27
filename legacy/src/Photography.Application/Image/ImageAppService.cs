using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Photography.Entities;
using Photography.Image.Dto;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace Photography.Image
{
    [AbpAuthorize]
    public class ImageAppService : AsyncCrudAppService<Entities.Image, ImageDto, Guid, ImagesPagedResultRequestDto, CreateImageDto, ImageDto>, IImageAppService
    {
        public ImageAppService(IRepository<Entities.Image, Guid> repository) : base(repository)
        {
        }

        protected override IQueryable<Entities.Image> CreateFilteredQuery(ImagesPagedResultRequestDto input)
        {
            return Repository.GetAll().Where(x => x.AlbumId.Equals(input.AlbumId));
        }

        public override async Task Delete(EntityDto<Guid> input)
        {
            CheckDeletePermission();

            ImageDto image = await Get(new EntityDto<Guid>(input.Id));

            string folderPath = Path.Combine(ImageExtentions.AbsoluteImagesFolder, image.AlbumId.ToString());
            string filePath = Path.Combine(folderPath, image.Id + Path.GetExtension(image.OriginalName));
            await Repository.DeleteAsync(input.Id);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task SetImageAsCover(Guid id)
        {
            CheckGetPermission();

            Entities.Image image = await Repository.GetAsync(id);
            List<Entities.Image> covers = await Repository.GetAll().Where(x => x.AlbumId == image.AlbumId && x.ImageType == ImageType.Cover).ToListAsync();

            foreach (Entities.Image cover in covers)
            {
                cover.ImageType = ImageType.Default;
                await Repository.UpdateAsync(cover);
            }

            image.ImageType = ImageType.Cover;
            await Repository.UpdateAsync(image);
        }

        public override async Task<ImageDto> Create(CreateImageDto input)
        {
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(input.File, new JpegDecoder()))
            {
                input.Height = image.Height;
                input.Width = image.Width;
                input.ImageOrientation = image.Width > image.Height ? ImageOrientation.Horizontal : ImageOrientation.Vertical;

                CheckCreatePermission();

                Entities.Image entity = MapToEntity(input);

                await Repository.InsertAsync(entity);
                await CurrentUnitOfWork.SaveChangesAsync();

                ImageDto imageDto = MapToEntityDto(entity);

                string folderPath = Path.Combine(ImageExtentions.AbsoluteImagesFolder, input.AlbumId.ToString());
                string filePath = Path.Combine(folderPath, imageDto.Id + Path.GetExtension(imageDto.OriginalName));

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                image.Save(filePath);

                return imageDto;
            }
        }

    }
}