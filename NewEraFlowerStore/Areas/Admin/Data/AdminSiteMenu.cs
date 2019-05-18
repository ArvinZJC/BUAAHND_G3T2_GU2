// csharp file that contains data configuration for the menu of the admin site

#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Data
{
    /// <summary>
    /// The class <see cref="AdminSiteMenu"/> contains data configuration for the menu of the admin site.
    /// </summary>
    public class AdminSiteMenu
    {
        /// <summary>
        /// It represents the item "Dashboard" in the menu of the admin site.
        /// </summary>
        public static string Dashboard => "Dashboard";
        /// <summary>
        /// It represents the item "Other administrators" in the menu of the admin site.
        /// </summary>
        public static string OtherAdministrators => "OtherAdministrators";
        /// <summary>
        /// It represents the item "Registered customers" in the menu of the admin site.
        /// </summary>
        public static string RegisteredCustomers => "RegisteredCustomers";
        /// <summary>
        /// It represents the item "Address books" in the menu of the admin site.
        /// </summary>
        public static string AddressBooks => "AddressBooks";
        /// <summary>
        /// It represents the item "Colours" in the menu of the admin site.
        /// </summary>
        public static string Colours => "Colours";
        /// <summary>
        /// It represents the item "Flowers" in the menu of the admin site.
        /// </summary>
        public static string Flowers => "Flowers";
        /// <summary>
        /// It represents the item "Occasions" in the menu of the admin site.
        /// </summary>
        public static string Occasions => "Occasions";
        /// <summary>
        /// It represents the item "Bouquets" in the menu of the admin site.
        /// </summary>
        public static string Bouquets => "Bouquets";
        /// <summary>
        /// It represents the item "Orders" in the menu of the admin site.
        /// </summary>
        public static string Orders => "Orders";
        /// <summary>
        /// It represents the item "Sales records" in the menu of the admin site.
        /// </summary>
        public static string SalesRecords => "SalesRecords";

        /// <summary>
        /// Validate whether the item "Dashboard" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string DashboardMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Dashboard);

        /// <summary>
        /// Validate whether the item "Other administrators" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string OtherAdministratorsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, OtherAdministrators);

        /// <summary>
        /// Validate whether the item "Registered customers" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string RegisteredCustomersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, RegisteredCustomers);

        /// <summary>
        /// Validate whether the item "Address books" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string AddressBooksMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, AddressBooks);

        /// <summary>
        /// Validate whether the item "Colours" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string ColoursMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Colours);

        /// <summary>
        /// Validate whether the item "Flowers" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string FlowersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Flowers);

        /// <summary>
        /// Validate whether the item "Occasions" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string OccasionsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Occasions);

        /// <summary>
        /// Validate whether the item "Bouquets" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string BouquetsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Bouquets);

        /// <summary>
        /// Validate whether the item "Orders" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string OrdersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Orders);

        /// <summary>
        /// Validate whether the item "Sales records" in the menu of the admin site is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string SalesRecordsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, SalesRecords);

        // validate whether the specified item in the menu of the admin site is active or not
        private static string PageMenuItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["AdminSitePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        } // end method PageMenuItem
    } // end class AdminSiteMenu
} // end namespace NewEraFlowerStore.Areas.Admin.Data