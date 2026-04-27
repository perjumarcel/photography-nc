using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Photography.Image.Dto;

namespace Photography.Readonly
{
    public class ImageAppService : IImageAppService
    {
        private readonly IRepository<Entities.Image, Guid> _repository;

        public ImageAppService(IRepository<Entities.Image, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ImageDto>> GetAll(Guid albumId)
        {
            var query = _repository.GetAll().Where(x => x.AlbumId == albumId).OrderBy(x => x.OriginalName.Length).ThenBy(x=>x.OriginalName);

            var entities = await query.ProjectTo<ImageDto>().ToListAsync();

            return entities;
        }
    }
}
