using Abp.Application.Navigation;
using Abp.Localization;
using Photography.Authorization;

namespace Photography.Web
{
    public class PhotographyNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu.AddItem(new MenuItemDefinition(PageNames.Frontend.Home, L("Home"), url: "/"))
                   .AddItem(new MenuItemDefinition(PageNames.Frontend.Portfolio, L("Portfolio"), url: "portfolio"))
                   //.AddItem(new MenuItemDefinition(PageNames.Frontend.Stories, L("Stories"), url: "stories"))
                   .AddItem(new MenuItemDefinition(PageNames.Frontend.About, L("About"), url: "about"))
                   .AddItem(new MenuItemDefinition(PageNames.Frontend.Contact, L("Contact"), url: "contact"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PhotographyConsts.LocalizationSourceName);
        }
    }
}
