using System.Collections.Generic;
using System.Linq;
using Photography.Entities;

namespace Photography.EntityFramework.SeedData.Base
{
    public class DefaultCategoryBuilder
    {
        private readonly List<string> _defaultList = new List<string>
                                                     {
                                                         "Wedding",
                                                         "Engagement",
                                                         "Family",
                                                         "Couple",
                                                         "Commercial",
                                                         "Baby",
                                                         "Event"
                                                     };

        private readonly PhotographyDbContext _context;

        public DefaultCategoryBuilder(PhotographyDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefault();
        }

        private void CreateDefault()
        {
            foreach (string item in _defaultList)
            {
                var defaultEntity = _context.Categories.FirstOrDefault(t => t.Title == item);
                if (defaultEntity == null)
                {
                    defaultEntity = new Category {Title = item, ShowAsFilter = false};

                    _context.Categories.Add(defaultEntity);
                    _context.SaveChanges();
                }
            }
        }
    }
}