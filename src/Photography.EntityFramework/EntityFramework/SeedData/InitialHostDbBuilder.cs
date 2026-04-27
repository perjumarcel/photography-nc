using EntityFramework.DynamicFilters;

namespace Photography.EntityFramework.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly PhotographyDbContext _context;

        public InitialHostDbBuilder(PhotographyDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
