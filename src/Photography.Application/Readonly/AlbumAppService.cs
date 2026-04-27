using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Photography.Album.Dto;

namespace Photography.Readonly
{
    public class AlbumAppService : IAlbumAppService
    {
        private readonly IRepository<Entities.Album, Guid> _repository;

        public AlbumAppService(IRepository<Entities.Album, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AlbumListItemDto>> GetAllHomeAlbumsAsync()
        {
            IQueryable<Entities.Album> query = _repository.GetAll();
            query = query.Where(x => x.ShowInHome);

            List<AlbumListItemDto> entities = await query.ProjectTo<AlbumListItemDto>().ToListAsync();

            return entities;
        }

        public async Task<IEnumerable<AlbumListItemDto>> GetAllStoriesAlbumsAsync()
        {
            IQueryable<Entities.Album> query = _repository.GetAll().Where(x => x.ShowInStories);

            List<AlbumListItemDto> entities = await query.ProjectTo<AlbumListItemDto>().ToListAsync();

            return entities;
        }

        public async Task<AlbumDto> Get(Guid id)
        {
            Entities.Album entity = await _repository.GetAll().Include(x => x.Category).SingleAsync(x => x.Id == id);

            return entity.MapTo<AlbumDto>();
        }

        public async Task<AlbumDto> GetNext(Guid albumId)
        {
            Entities.Album album = _repository.Get(albumId);
            Entities.Album entity = await _repository.GetAll().OrderBy(x => x.CreationTime).FirstOrDefaultAsync(x => x.CreationTime > album.CreationTime) ?? 
                                    await _repository.GetAll().OrderBy(x => x.CreationTime).FirstOrDefaultAsync();


            return entity.MapTo<AlbumDto>();
        }

        public async Task<AlbumDto> GetPrevious(Guid albumId)
        {
            Entities.Album album = _repository.Get(albumId);
            Entities.Album entity = await _repository.GetAll().OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync(x => x.CreationTime < album.CreationTime) ??
                                    await _repository.GetAll().OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync();

            return entity.MapTo<AlbumDto>();
        }

        public async Task<IEnumerable<AlbumListItemDto>> GetAllPortfoliosAsync()
        {
            IQueryable<Entities.Album> query = _repository.GetAll().Where(x => x.ShowInPortfolio);
            List<AlbumListItemDto> entities = await query.ProjectTo<AlbumListItemDto>().ToListAsync();

            return entities;
        }
    }
}
