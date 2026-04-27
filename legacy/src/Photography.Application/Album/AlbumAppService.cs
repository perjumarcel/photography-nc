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
using AutoMapper.QueryableExtensions;
using Photography.Album.Dto;
using Photography.Image;

namespace Photography.Album
{
    [AbpAuthorize]
    public class AlbumAppService : AsyncCrudAppService<Entities.Album, AlbumDto, Guid, PagedResultRequestDto, CreateAlbumDto, AlbumDto>, IAlbumAppService
    {
        public AlbumAppService(IRepository<Entities.Album, Guid> repository) : base(repository)
        {
        }

        public override async Task Delete(EntityDto<Guid> input)
        {
            CheckDeletePermission();

            await Repository.DeleteAsync(input.Id);

            string albumFolder = Path.Combine(ImageExtentions.AbsoluteImagesFolder, input.Id.ToString());
            if (Directory.Exists(albumFolder))
            {
                Directory.Delete(albumFolder, true);
            }
        }


        public virtual async Task<AlbumDto> Update(AlbumDto input)
        {
            try
            {
                CheckUpdatePermission();

                var entity = await GetEntityByIdAsync(input.Id);

                MapToEntity(input, entity);
                await CurrentUnitOfWork.SaveChangesAsync();

                return MapToEntityDto(entity);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }



        public async Task<IEnumerable<AlbumDdDto>> GetAllKeyValuesAsync()
        {
            CheckGetAllPermission();

            IQueryable<Entities.Album> query = Repository.GetAll();

            List<AlbumDdDto> entities = await query.ProjectTo<AlbumDdDto>().ToListAsync();

            return entities;
        }
    }
}
