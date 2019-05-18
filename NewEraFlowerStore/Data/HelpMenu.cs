// csharp file that contains data configuration for the menu of the item "Help" in the header navigation

#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="HelpMenu"/> contains data configuration for the menu of the item "Help" in the header navigation.
    /// </summary>
    public static class HelpMenu
    {
        /// <summary>
        /// It represents the item "FAQ" in the menu of the item "Help" in the header navigation.
        /// </summary>
        public static string Faq => "Faq";
        /// <summary>
        /// It represents the item "Privacy policy" in the menu of the item "Help" in the header navigation.
        /// </summary>
        public static string PrivacyPolicy => "PrivacyPolicy";
        /// <summary>
        /// It represents the item "User agreement" in the menu of the item "Help" in the header navigation.
        /// </summary>
        public static string UserAgreement => "UserAgreement";
        /// <summary>
        /// It represents the item "About us" in the menu of the item "Help" in the header navigation.
        /// </summary>
        public static string AboutUs => "AboutUs";
        /// <summary>
        /// It represents the item "Contact us" in the menu of the item "Help" in the header navigation.
        /// </summary>
        public static string ContactUs => "ContactUs";

        /// <summary>
        /// Validate whether the item "FAQ" in the menu of the item "Help" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string FaqMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Faq);

        /// <summary>
        /// Validate whether the item "Privacy policy" in the menu of the item "Help" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string PrivacyPolicyMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, PrivacyPolicy);

        /// <summary>
        /// Validate whether the item "User agreement" in the menu of the item "Help" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string UserAgreementMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, UserAgreement);

        /// <summary>
        /// Validate whether the item "About us" in the menu of the item "Help" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string AboutUsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, AboutUs);

        /// <summary>
        /// Validate whether the item "Contact us" in the menu of the item "Help" in the header navigation is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <returns>a string "active" or null</returns>
        public static string ContactUsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, ContactUs);

        // validate whether the specified item in the menu of the item "Help" in the header navigation is active or not
        private static string PageMenuItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["HelpPage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        } // end method PageMenuItem
    } // end class HelpMenu
} // end namespace NewEraFlowerStore.Data