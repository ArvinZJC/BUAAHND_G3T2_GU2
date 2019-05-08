#region Using Directives
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class Flower
    {
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Flower", AdditionalFields = nameof(Name))]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        [DataType(DataType.Text)]
        [StringLength(25, ErrorMessage = "Please enter a valid name.")] // the relevant regular expression and tooltip need updating after modifying the length
        /* 
         * the length here should be equal to the relevant attribute;
         * at max 25 characters long, with a digit or an uppercase letter as the 1st character;
         * only "&", "-", ".", and spaces are non-alphanumeric characters allowed
         */
        [RegularExpression(@"^[0-9A-Z][&-.\s0-9A-Za-z]{0,24}$", ErrorMessage = "Please enter a valid name.")]
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Flower", AdditionalFields = nameof(ID))]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        [DataType(DataType.Text)]
        [StringLength(300, ErrorMessage = "Please enter a valid description.")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string CoverPhotoUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<Bouquet> Bouquets { get; set; }
    } // end class Flower
} // end namespace NewEraFlowerStore.Data