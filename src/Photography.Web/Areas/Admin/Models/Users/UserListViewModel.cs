using System.Collections.Generic;
using Photography.Roles.Dto;
using Photography.Users.Dto;

namespace Photography.Web.Admin.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}