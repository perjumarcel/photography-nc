using Abp.Authorization;
using Photography.Authorization.Roles;
using Photography.Authorization.Users;

namespace Photography.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
