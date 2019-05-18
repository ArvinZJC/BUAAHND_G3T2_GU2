// csharp file that contains data properties for a flower

#region Using Directives
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="Flower"/> contains data properties for a flower.
    /// </summary>
    public class Flower
    {
        /// <summary>
        /// ID of a flower.
        /// </summary>
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Flower", AdditionalFields = nameof(Name))]
        public int ID { get; set; }
        /// <summary>
        /// A flower name.
        /// </summary>
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
        /// <summary>
        /// A flower description.
        /// </summary>
        [Required(ErrorMessage = "Please enter a description.")]
        [DataType(DataType.Text)]
        [StringLength(300, ErrorMessage = "Please enter a valid description.")]
        public string Description { get; set; }
        /// <summary>
        /// The URL of a cover photo for a flower.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        public string CoverPhotoUrl { get; set; }
        [JsonIgnore]
        public virtual ICollection<Bouquet> Bouquets { get; set; }
    } // end class Flower
} // end namespace NewEraFlowerStore.Data