using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Photography.EntityFramework.Repositories
{
    public abstract class PhotographyRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<PhotographyDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected PhotographyRepositoryBase(IDbContextProvider<PhotographyDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class PhotographyRepositoryBase<TEntity> : PhotographyRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected PhotographyRepositoryBase(IDbContextProvider<PhotographyDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
