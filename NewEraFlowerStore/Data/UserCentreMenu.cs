#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public static class UserCentreMenu
    {
        public static string UserCentre => "UserCentre";

        public static string Profile => "Profile";

        public static string ChangePassword => "ChangePassword";

        public static string AddressBooks => "AddressBooks";

        public static string Orders => "Orders";

        public static string DeactivateAccount => "DeletePersonalData";

        public static string UserCentreMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, UserCentre);

        public static string ProfileMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Profile);

        public static string ChangePasswordMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, ChangePassword);

        public static string AddressBooksMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, AddressBooks);

        public static string OrdersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Orders);

        public static string DeactivateAccountMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, DeactivateAccount);

        private static string PageMenuItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["UserCentrePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        } // end method PageMenuItem
    } // end class UserCentreMenu
} // end namespace NewEraFlowerStore.Data