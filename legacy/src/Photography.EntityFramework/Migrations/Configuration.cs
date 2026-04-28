using System.Data.Entity.Migrations;
using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using EntityFramework.DynamicFilters;
using Photography.EntityFramework.SeedData;
using Photography.EntityFramework.SeedData.Base;

namespace Photography.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<Photography.EntityFramework.PhotographyDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Photography";
        }

        protected override void Seed(Photography.EntityFramework.PhotographyDbContext context)
        {
            context.DisableAllFilters();

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                //new TenantRoleAndUserBuilder(context, 1).Create();
                new DefaultCategoryBuilder(context).Create();
            }
            else
            {
                //You can add seed for tenant databases and use Tenant property...
            }

            context.SaveChanges();
        }
    }
}
