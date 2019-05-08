#region Using Directives
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public static class BouquetCategoryMenu
    {
        public static string FlowerMenuItem(ViewContext viewContext, string flowerName)
        {
            if (!string.IsNullOrWhiteSpace(flowerName))
            {
                var activePage = (string)viewContext.ViewData["FlowerPage"];

                if (activePage != null)
                    return string.Equals(activePage, "Flowers - " + flowerName.ToLower(), StringComparison.OrdinalIgnoreCase) ? "active" : null;
            }  

            return null;
        } // end method FlowerMenuItem

        public static string OccasionMenuItem(ViewContext viewContext, string occasionName)
        {
            if (!string.IsNullOrWhiteSpace(occasionName))
            {
                var activePage = (string)viewContext.ViewData["OccasionPage"];

                if (activePage != null)
                    return string.Equals(activePage, "Occasions - " + occasionName.ToLower(), StringComparison.OrdinalIgnoreCase) ? "active" : null;
            }

            return null;
        } // end method FlowerMenuItem
    } // end class BouquetCategoryMenu
} // end namespace NewEraFlowerStore.Data