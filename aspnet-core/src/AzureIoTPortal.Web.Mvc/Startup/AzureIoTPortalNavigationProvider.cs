using Abp.Application.Navigation;
using Abp.Localization;
using AzureIoTPortal.Authorization;

namespace AzureIoTPortal.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class AzureIoTPortalNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Home,
                        L("Boards"),
                        url: "/iot/index",
                        icon: "home",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Devices"),
                        url: "/iot/devices",
                        icon: "business",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("FloorPlan"),
                        url: "/iot/floorplan",
                        icon: "business",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Alarms"),
                        url: "/iot/alarms",
                        icon: "business",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Rules"),
                        url: "/iot/rules",
                        icon: "business",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "people",
                        requiredPermissionName: PermissionNames.Pages_Users
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Roles,
                        L("Roles"),
                        url: "Roles",
                        icon: "local_offer",
                        requiredPermissionName: PermissionNames.Pages_Roles
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Audit"),
                        url: "/iot/audit",
                        icon: "business",
                        requiredPermissionName: PermissionNames.Pages_Audit
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Reports"),
                        url: "/iot/reports",
                        icon: "business",
                        requiredPermissionName: PermissionNames.Pages_Reports
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("Settings"),
                        url: "/iot/settings",
                        icon: "info"
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AzureIoTPortalConsts.LocalizationSourceName);
        }
    }
}
