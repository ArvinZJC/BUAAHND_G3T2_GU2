// csharp file that contains data configuration for the menu of the user centre

#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="UserCentreMenu"/> contains data configuration for the menu of the user centre.
    /// </summary>
    public static class UserCentreMenu
    {
        /// <summary>
        /// It represents the item "User centre" in the menu of the user centre.
        /// </summary>
        public static string UserCentre => "UserCentre";
        /// <summary>
        /// It represents the item "Profile" in the menu of the user centre.
        /// </summary>
        public static string Profile => "Profile";
        /// <summary>
        /// It represents the item "Change password" in the menu of the user centre.
        /// </summary>
        public static string ChangePassword => "ChangePassword";
        /// <summary>
        /// It represents the item "Address books" in the menu of the user centre.
        /// </summary>
        public static string AddressBooks => "AddressBooks";
        /// <summary>
        /// It represents the item "Orders" in the menu of the user centre.
        /// </summary>
        public static string Orders => "Orders";
        /// <summary>
        /// It represents the item "Deactivate account" in the menu of the user centre.
        /// </summary>
        public static string DeactivateAccount => "DeletePersonalData";

        /// <summary>
        /// Validate whether the item "User centre" in the menu of the user centre is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string UserCentreMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, UserCentre);

        /// <summary>
        /// Validate whether the item "Profile" in the menu of the user centre is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string ProfileMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Profile);

        /// <summary>
        /// Validate whether the item "Change password" in the menu of the user centre is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string ChangePasswordMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, ChangePassword);

        /// <summary>
        /// Validate whether the item "Address books" in the menu of the user centre is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string AddressBooksMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, AddressBooks);

        /// <summary>
        /// Validate whether the item "Orders" in the menu of the user centre is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string OrdersMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Orders);

        /// <summary>
        /// Validate whether the item "Deactivate account" in the menu of the user centre is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string DeactivateAccountMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, DeactivateAccount);

        // validate whether the specified item in the menu of the user centre is active or not
        private static string PageMenuItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["UserCentrePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        } // end method PageMenuItem
    } // end class UserCentreMenu
} // end namespace NewEraFlowerStore.Data