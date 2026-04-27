using Abp;
using Abp.Application.Navigation;
using Abp.Localization;
using Photography.Authorization;

namespace Photography.Web
{
    public class PhotographyAdminNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.Menus.Add("AdminMenu", new MenuDefinition("AdminMenu", new LocalizableString("AdminMenu", AbpConsts.LocalizationSourceName)));
            context.Manager.Menus["AdminMenu"]
                   .AddItem(
                       new MenuItemDefinition(
                           PageNames.Admin.Albums,
                           L("Albums"),
                           url: "admin/Albums",
                           icon: "perm_media",
                           requiresAuthentication: true
                       ))
                   .AddItem(
                       new MenuItemDefinition(
                           PageNames.Admin.Images,
                           L("Images"),
                           url: "admin/Images",
                           icon: "collections",
                           requiresAuthentication: true
                       ))
                   .AddItem(
                       new MenuItemDefinition(
                           PageNames.Admin.Categories,
                           L("Categories"),
                           url: "admin/Categories",
                           icon: "view_comfy",
                           requiresAuthentication: true
                       ))
                   //.AddItem(
                   // new MenuItemDefinition(
                   //     PageNames.Admin.Tenants,
                   //     L("Tenants"),
                   //     url: "admin/Tenants",
                   //     icon: "business",
                   //     requiredPermissionName: PermissionNames.Pages_Tenants
                   // ))
                   .AddItem(
                    new MenuItemDefinition(
                        PageNames.Admin.Users,
                        L("Users"),
                        url: "admin/Users",
                        icon: "people",
                        requiredPermissionName: PermissionNames.Pages_Users
                    ))
                   .AddItem(
                    new MenuItemDefinition(
                        PageNames.Admin.Roles,
                        L("Roles"),
                        url: "admin/Roles",
                        icon: "local_offer",
                        requiredPermissionName: PermissionNames.Pages_Roles
                    ))
                   .AddItem(
                       new MenuItemDefinition(
                           PageNames.Admin.Dashboard,
                           L("HomePage"),
                           url: "",
                           icon: "home",
                           requiresAuthentication: true
                       ));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PhotographyConsts.LocalizationSourceName);
        }
    }
}
