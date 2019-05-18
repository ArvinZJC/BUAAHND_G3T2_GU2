// csharp file that contains data configuration for a gender list

using System.Collections.Generic;

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="GenderListItem"/> contains data configuration for a gender list.
    /// </summary>
    public class GenderListItem
    {
        /// <summary>
        /// A display name of an item in a gender list.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// ID of an item in a gender list.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Get a gender list.
        /// </summary>
        /// <returns>a gender list</returns>
        public List<GenderListItem> GetGenderList()
        {
            List<GenderListItem> genderList = new List<GenderListItem>
            {
                new GenderListItem() { DisplayName = "Male", ID = 1 },
                new GenderListItem() { DisplayName = "Female", ID = 2 }
            };
            return genderList;
        } // end method GetGenderList

        /// <summary>
        /// Validate whether the specified ID is valid or not.
        /// </summary>
        /// <param name="ID">ID of an item in a gender list</param>
        /// <returns>a Boolean value (true or false)</returns>
        public bool IsValidId(int? ID)
        {
            switch (ID)
            {
                case null:
                case 1:
                case 2:
                    return true;

                default:
                    return false;
            } // end switch-case
        } // end method IsValidId
    } // end class GenderListItem
} // end namespace NewEraFlowerStore.Data