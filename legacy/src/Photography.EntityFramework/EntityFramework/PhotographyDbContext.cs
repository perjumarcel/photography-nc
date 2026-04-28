using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Photography.Authorization.Roles;
using Photography.Authorization.Users;
using Photography.Entities;
using Photography.MultiTenancy;

namespace Photography.EntityFramework
{
    public class PhotographyDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        //public virtual DbSet<SitePage> SitePages { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public PhotographyDbContext()
            : base("LocalSqlServer")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in PhotographyDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of PhotographyDbContext since ABP automatically handles it.
         */
        public PhotographyDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public PhotographyDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public PhotographyDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
