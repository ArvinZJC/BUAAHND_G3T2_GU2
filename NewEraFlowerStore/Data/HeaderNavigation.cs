// csharp file that contains data configuration for the header navigation

#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="HeaderNavigation"/> contains data configuration for the header navigation.
    /// </summary>
    public static class HeaderNavigation
    {
        /// <summary>
        /// It represents the item "Bouquets" in the header navigation.
        /// </summary>
        public static string Bouquets => "Bouquets";
        /// <summary>
        /// It represents the item "Flowers" in the header navigation.
        /// </summary>
        public static string Flowers => "Flowers";
        /// <summary>
        /// It represents the item "Occasions" in the header navigation.
        /// </summary>
        public static string Occasions => "Occasions";
        /// <summary>
        /// It represents the item "Help" in the header navigation.
        /// </summary>
        public static string Help => "Help";
        /// <summary>
        /// It represents the item "Register" in the header navigation.
        /// </summary>
        public static string Register => "Register";
        /// <summary>
        /// It represents the item "Log in" in the header navigation.
        /// </summary>
        public static string Login => "Login";
        /// <summary>
        /// It represents the item "Cart" in the header navigation.
        /// </summary>
        public static string Cart => "Cart";
        /// <summary>
        /// It represents the item "Admin site" in the header navigation.
        /// </summary>
        public static string AdminSite => "AdminSite";
        /// <summary>
        /// It represents the item "User centre" in the header navigation.
        /// </summary>
        public static string UserCentre => "UserCentre";

        /// <summary>
        /// Validate whether the item "Bouquets" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string BouquetsNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Bouquets);

        /// <summary>
        /// Validate whether the item "Flowers" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string FlowersNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Flowers);

        /// <summary>
        /// Validate whether the item "Occasions" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string OccasionsNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Occasions);

        /// <summary>
        /// Validate whether the item "Help" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string HelpNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Help);

        /// <summary>
        /// Validate whether the item "Register" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string RegisterNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Register);

        /// <summary>
        /// Validate whether the item "Log in" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string LoginNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Login);

        /// <summary>
        /// Validate whether the item "Cart" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string CartNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, Cart);

        /// <summary>
        /// Validate whether the item "Admin site" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string AdminSiteNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, AdminSite);

        /// <summary>
        /// Validate whether the item "User centre" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active text-success" or null</returns>
        public static string UserCentreNavigationItem(ViewContext viewContext) => PageNavigationItem(viewContext, UserCentre);

        // validate whether the specified item in the header navigation is active or not
        private static string PageNavigationItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["StorePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active text-success" : null;
        } // end method PageNavigationItem
    } // end class HeaderNavigation
} // end namespace NewEraFlowerStore.Data