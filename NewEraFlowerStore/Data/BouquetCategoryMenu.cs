// csharp file that contains data configuration for the menu of bouquets

#region Using Directives
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="BouquetCategoryMenu"/> contains data configuration for the menu of bouquets.
    /// </summary>
    public static class BouquetCategoryMenu
    {
        /// <summary>
        /// Validate whether the specified item in the menu of flowers for a bouquet is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <param name="flowerName">a flower name</param>
        /// <returns>a string "active" or null</returns>
        public static string FlowerMenuItem(ViewContext viewContext, string flowerName)
        {
            if (!string.IsNullOrWhiteSpace(flowerName))
            {
                var activePage = (string)viewContext.ViewData["FlowerPage"];

                if (activePage != null)
                    return string.Equals(activePage, "Flowers - " + flowerName.ToLower(), StringComparison.OrdinalIgnoreCase) ? "active" : null;
            } // end if

            return null;
        } // end method FlowerMenuItem

        /// <summary>
        /// Validate whether the specified item in the menu of occasions for a bouquet is active or not.
        /// </summary>
        /// <param name="viewContext">context for view execution</param>
        /// <param name="occasionName">an occasion name</param>
        /// <returns>a string "active" or null</returns>
        public static string OccasionMenuItem(ViewContext viewContext, string occasionName)
        {
            if (!string.IsNullOrWhiteSpace(occasionName))
            {
                var activePage = (string)viewContext.ViewData["OccasionPage"];

                if (activePage != null)
                    return string.Equals(activePage, "Occasions - " + occasionName.ToLower(), StringComparison.OrdinalIgnoreCase) ? "active" : null;
            } // end if

            return null;
        } // end method FlowerMenuItem
    } // end class BouquetCategoryMenu
} // end namespace NewEraFlowerStore.Data