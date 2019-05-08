using System.Collections.Generic;

namespace NewEraFlowerStore.Data
{
    public class GenderListItem
    {
        public string DisplayName { get; set; }

        public int ID { get; set; }

        public List<GenderListItem> GetGenderList()
        {
            List<GenderListItem> genderList = new List<GenderListItem>
            {
                new GenderListItem() { DisplayName = "Male", ID = 1 },
                new GenderListItem() { DisplayName = "Female", ID = 2 }
            };
            return genderList;
        } // end method GetGenderList

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