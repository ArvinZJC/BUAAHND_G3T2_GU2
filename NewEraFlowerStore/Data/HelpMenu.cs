#region Using Directives
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public static class HelpMenu
    {
        public static string Faq => "Faq";

        public static string PrivacyPolicy => "PrivacyPolicy";

        public static string UserAgreement => "UserAgreement";

        public static string AboutUs => "AboutUs";

        public static string ContactUs => "ContactUs";

        public static string FaqMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, Faq);

        public static string PrivacyPolicyMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, PrivacyPolicy);

        public static string UserAgreementMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, UserAgreement);

        public static string AboutUsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, AboutUs);

        public static string ContactUsMenuItem(ViewContext viewContext) => PageMenuItem(viewContext, ContactUs);

        private static string PageMenuItem(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["HelpPage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        } // end method PageMenuItem
    } // end class HelpMenu
} // end namespace NewEraFlowerStore.Data