using System.Collections.Generic;
using System.Web.Mvc;
using Photography.Roles.Dto;

namespace Photography.Web.Admin.Models.Categories
{
    public class CategoryListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }

        public List<SelectListItem> Albums { get; set; }
    }
}
