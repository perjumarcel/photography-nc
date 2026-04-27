using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Microsoft.AspNet.Identity;
using Photography.Authorization;
using Photography.Authorization.Roles;
using Photography.Authorization.Users;

namespace Photography.EntityFramework.SeedData
{
    public class HostRoleAndUserCreator
    {
        private readonly PhotographyDbContext _context;

        public HostRoleAndUserCreator(PhotographyDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = _context.Roles.FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {

                adminRoleForHost = new Role
                {
                    Name = StaticRoleNames.Host.Admin,
                    DisplayName = StaticRoleNames.Host.Admin,
                    IsStatic = true
                };

                adminRoleForHost.SetNormalizedName();

                _context.Roles.Add(adminRoleForHost);
                _context.SaveChanges();

                //Grant all tenant permissions
                var permissions = PermissionFinder
                    .GetAllPermissions(new PhotographyAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host))
                    .ToList();

                foreach (var permission in permissions)
                {
                    _context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRoleForHost.Id
                        });
                }

                _context.SaveChanges();
            }

            //Admin user for tenancy host

            var adminUserForHost = _context.Users.FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost != null)
            {
                return;
            }

            adminUserForHost = _context.Users.Add(
                new User
                {
                    UserName = AbpUserBase.AdminUserName,
                    Name = "Nicolae",
                    Surname = "Covercenco",
                    EmailAddress = "nickcovercenco@yahoo.com",
                    IsEmailConfirmed = true,
                    Password = new PasswordHasher().HashPassword(User.DefaultPassword)
                });

            User adminUserForHost2 = _context.Users.Add(
                new User
                {
                    UserName = "mperju",
                    Name = "Marcel",
                    Surname = "Perju",
                    EmailAddress = "perjumarcel@yahoo.com",
                    IsEmailConfirmed = true,
                    Password = new PasswordHasher().HashPassword("hx&Z+^LFRFbbd=")
                });

            adminUserForHost.SetNormalizedNames();
            adminUserForHost2.SetNormalizedNames();

            _context.SaveChanges();

            // Assign Admin role to admin user
            _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
            _context.UserRoles.Add(new UserRole(null, adminUserForHost2.Id, adminRoleForHost.Id));
        }
    }
}