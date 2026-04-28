using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Photography.EntityFramework;

namespace Photography.Migrator
{
    [DependsOn(typeof(PhotographyDataModule))]
    public class PhotographyMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<PhotographyDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}