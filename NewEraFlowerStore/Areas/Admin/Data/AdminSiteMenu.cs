#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Data
{
    public class AdminSiteMenu
    {
        public static string Dashboard => "Dashboard";

        public static string OtherAdministrators => "OtherAdministrators";

        public static string RegisteredCustomers => "RegisteredCustomers";

        public static string AddressBooks => "AddressBooks";

        public static string Colours => "Colours";

        public static string Flowers => "Flowers";

        public static string Occasions => "Occasions";

        public static string Bouquets => "Bouquets";

        public static string Orders => "Orders";

        public static string SalesRecords => "SalesRecords";

        public static string DashboardMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Dashboard);

        public static string OtherAdministratorsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, OtherAdministrators);

        public static string RegisteredCustomersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, RegisteredCustomers);

        public static string AddressBooksMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, AddressBooks);

        public static string ColoursMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Colours);

        public static string FlowersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Flowers);

        public static string OccasionsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Occasions);

        public static string BouquetsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Bouquets);

        public static string OrdersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Orders);

        public static string SalesRecordsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, SalesRecords);

        private static string PageMenuItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["AdminSitePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        } // end method PageMenuItem
    } // end class AdminSiteMenu
} // end namespace NewEraFlowerStore.Areas.Admin.Data