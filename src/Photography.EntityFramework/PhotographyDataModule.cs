using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using Photography.EntityFramework;

namespace Photography
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(PhotographyCoreModule))]
    public class PhotographyDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PhotographyDbContext>());

            Configuration.DefaultNameOrConnectionString = "LocalSqlServer";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
