using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using Photography.Album;
using Photography.Album.Dto;
using Photography.Entities;
using Photography.Image;
using Photography.Image.Dto;
using Photography.Web.Admin.Models.FineUploader;
using Photography.Web.Admin.Models.Images;
using Photography.Web.Controllers;

namespace Photography.Web.Admin.Controllers
{
    [AbpMvcAuthorize]
    public class ImagesController : PhotographyControllerBase
    {
        private readonly IAlbumAppService _albumAppService;
        private readonly IImageAppService _imageAppService;

        public ImagesController(IImageAppService imageAppService, IAlbumAppService albumAppService)
        {
            _imageAppService = imageAppService;
            _albumAppService = albumAppService;
        }

        public async Task<ActionResult> Index(GeAllImagesInput input)
        {
            var albums = (await _albumAppService.GetAllKeyValuesAsync()).ToList();

            if (input.AlbumId == Guid.Empty && albums.Any())
            {
                input.AlbumId = albums.First().Id;
            }

            PagedResultDto<ImageDto> images = input.AlbumId == Guid.Empty
                                                  ? new PagedResultDto<ImageDto>()
                                                  : await _imageAppService.GetAll(
                                                        new ImagesPagedResultRequestDto
                                                        {
                                                            MaxResultCount = int.MaxValue,
                                                            Sorting = "OriginalName",
                                                            AlbumId = input.AlbumId
                                                        });

            // set first as Cover if any Horizontal image exists
            if (images.Items.Any(x => x.ImageOrientation == ImageOrientation.Horizontal) && images.Items.All(x => x.ImageType != ImageType.Cover))
            {
                ImageDto first = images.Items.First(x => x.ImageOrientation == ImageOrientation.Horizontal);
                await _imageAppService.SetImageAsCover(first.Id);
                first.ImageType = ImageType.Cover;
            }

            var model = new ImageListViewModel
            {
                Images = images.Items.OrderBy(x=>x.OriginalName.Length).ThenBy(x=>x.OriginalName).ToList(),
                Albums = albums,
                AlbumId = input.AlbumId != Guid.Empty ? input.AlbumId : albums.Any() ? albums.First().Id : Guid.Empty
            };

            return View(model);
        }

        public async Task<ActionResult> UploadImageModal(Guid albumId)
        {
            AlbumDto albumDto = await _albumAppService.Get(new EntityDto<Guid>(albumId));
            return View("_UploadImageModal", albumDto);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<FineUploaderResult> Upload(FineUpload fineUpload, Guid albumId)
        {
            if (fineUpload.InputStream.Length <= 0)
            {
                throw new FileLoadException();
            }

            ImageDto dbImage;
            var input = new CreateImageDto
            {
                AlbumId = albumId,
                OriginalName = fineUpload.Filename,
                File = fineUpload.InputStream
            };
            try
            {
                dbImage = await _imageAppService.Create(input);
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            return new FineUploaderResult(true, new { fileid = dbImage.Id });
        }
    }
}