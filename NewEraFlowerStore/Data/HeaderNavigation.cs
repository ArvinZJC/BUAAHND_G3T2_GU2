#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public static class HeaderNavigation
    {
        public static string Bouquets => "Bouquets";

        public static string Flowers => "Flowers";

        public static string Occasions => "Occasions";

        public static string Help => "Help";

        public static string Register => "Register";

        public static string Login => "Login";

        public static string Cart => "Cart";

        public static string AdminSite => "AdminSite";

        public static string UserCentre => "UserCentre";

        public static string BouquetsNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Bouquets);

        public static string FlowersNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Flowers);

        public static string OccasionsNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Occasions);

        public static string HelpNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Help);

        public static string RegisterNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Register);

        public static string LoginNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Login);

        public static string CartNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Cart);

        public static string AdminSiteNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, AdminSite);

        public static string UserCentreNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, UserCentre);

        private static string PageNavigationItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["StorePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active text-success" : null;
        } // end method PageNavigationItem
    } // end class HeaderNavigation
} // end namespace NewEraFlowerStore.Data